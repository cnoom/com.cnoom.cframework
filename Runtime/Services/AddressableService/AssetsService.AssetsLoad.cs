using System;
using System.Collections;
using System.Collections.Generic;
using CnoomFrameWork.Base;
using CnoomFrameWork.Base.Log;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace CnoomFrameWork.Modules.AddressableModule
{
    public partial class AssetsService
    {
        // 同步加载（慎用，可能造成卡顿）
        public T LoadAsset<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                LogManager.Error(nameof(AssetsService), "资源键无效!");
                return default;
            }

            try
            {
                var locationHandle = Addressables.LoadResourceLocationsAsync(key, typeof(T));
                locationHandle.WaitForCompletion();

                if (locationHandle.Status != AsyncOperationStatus.Succeeded || locationHandle.Result.Count <= 0)
                {
                    LogManager.Error(nameof(AssetsService), $"加载失败: 无法找到地址键 {key}");
                    return default;
                }

                var location = locationHandle.Result[0];
                if (assetHandles.TryGetValue(location.PrimaryKey, out var assetHandle))
                {
                    referenceCount[location.PrimaryKey]++;
                    return (T)assetHandle.Result;
                }

                var operation = Addressables.LoadAssetAsync<T>(location);
                TrackHandle(location.PrimaryKey, operation);
                var result = operation.WaitForCompletion();

                return result;
            }
            catch (ResourceException e)
            {
                LogManager.Error(nameof(AssetsService), $"加载失败: {e.Message}");
                return default;
            }
        }

        /// <summary>
        ///     异步加载资源
        /// </summary>
        public void LoadAssetAsync<T>(string key, Action<T> onLoaded)
        {
            if (string.IsNullOrEmpty(key))
            {
                LogManager.Error(nameof(AssetsService), "资源键无效!");
                return;
            }

            app.StartCoroutine(LoadAssetAsyncRoutine(key, onLoaded));
        }

        public IEnumerator LoadAssetAsyncRoutine<T>(string key, Action<T> onLoaded)
        {
            IResourceLocation location = null;
            yield return LocateResource<T>(key, locatedLocation => location = locatedLocation[0]);
            var primaryKey = location.PrimaryKey;
            if (assetHandles.TryGetValue(primaryKey, out var assetHandle))
            {
                referenceCount[primaryKey]++;
                onLoaded?.Invoke((T)assetHandle.Result);
                yield break;
            }

            while (loadingHandles.ContainsKey(primaryKey))
            {
                yield return loadingHandles[primaryKey];
                if (!assetHandles.TryGetValue(primaryKey, out var existingHandle)) continue;
                referenceCount[primaryKey]++;
                onLoaded?.Invoke((T)existingHandle.Result);
                yield break;
            }

            var operation = Addressables.LoadAssetAsync<T>(location);
            loadingHandles[primaryKey] = operation;
            TrackHandle(primaryKey, operation);
            yield return operation;
            loadingHandles.Remove(primaryKey);
            if (operation.Status == AsyncOperationStatus.Succeeded)
            {
                onLoaded?.Invoke(operation.Result);
            }
            else
            {
                LogManager.Error(nameof(AssetsService), $"加载失败: {key} [{operation.OperationException}]");
                Addressables.Release(operation); // 新增释放操作
            }
        }

        /// <summary>
        ///     通过AssetReference加载资源
        /// </summary>
        public void LoadByReference<T>(AssetReference reference, Action<T> onLoaded)
        {
            if (!reference.RuntimeKeyIsValid())
            {
                LogManager.Error(nameof(AssetsService), "无效的AssetReference");
                return;
            }

            app.StartCoroutine(LoadAssetByReferenceCoroutine(reference, onLoaded));
        }

        public IEnumerator LoadAssetByReferenceCoroutine<T>(AssetReference reference, Action<T> onLoaded)
        {
            IResourceLocation location = null;
            yield return LocateResource<T>(reference, locatedLocation => location = locatedLocation[0]);
            if (assetHandles.TryGetValue(location.PrimaryKey, out var assetHandle))
            {
                referenceCount[location.PrimaryKey]++;
                onLoaded?.Invoke((T)assetHandle.Result);
                yield break;
            }

            var operation = Addressables.LoadAssetAsync<T>(location);
            TrackHandle(location.PrimaryKey, operation);
            yield return operation;

            if (operation.Status != AsyncOperationStatus.Succeeded)
            {
                LogManager.Error(nameof(AssetsService), $"加载失败: {reference.AssetGUID} [{operation.OperationException}]");
                yield break;
            }

            onLoaded?.Invoke(operation.Result);
        }

        /// <summary>
        ///     通过标签加载多个资源
        /// </summary>
        public void LoadAssetsByLabel<T>(string label, Action<T> onLoaded, Action<float> onProgress = null)
        {
            app.StartCoroutine(LoadAssetsByLabelCoroutine(label, onLoaded, onProgress));
        }

        public IEnumerator LoadAssetsByLabelCoroutine<T>(string label, Action<T> onLoaded,
            Action<float> onProgress = null)
        {
            IList<IResourceLocation> locations = null;
            yield return LocateResource<T>(label, locatedLocations => locations = locatedLocations);

            var totalProgress = 0f;
            for (var i = 0; i < locations.Count; i++)
            {
                var location = locations[i];
                if (assetHandles.TryGetValue(location.PrimaryKey, out var assetHandle))
                {
                    referenceCount[location.PrimaryKey]++;
                    onLoaded?.Invoke((T)assetHandle.Result);
                    continue;
                }

                var operation = Addressables.LoadAssetAsync<T>(location);
                TrackHandle(location.PrimaryKey, operation);

                while (!operation.IsDone)
                {
                    totalProgress = (i + operation.PercentComplete) / locations.Count;
                    onProgress?.Invoke(totalProgress);
                    yield return null;
                }

                if (operation.Status != AsyncOperationStatus.Succeeded)
                {
                    LogManager.Error(nameof(AssetsService),$"加载失败: {location.PrimaryKey} [{operation.OperationException}]");
                    continue;
                }

                onLoaded?.Invoke(operation.Result);
            }
        }

        private IEnumerator LocateResource<T>(string key, Action<IList<IResourceLocation>> onLocated)
        {
            var locationHandle = Addressables.LoadResourceLocationsAsync(key, typeof(T));
            yield return locationHandle;
            LoadResource(key, locationHandle, onLocated);
        }

        private IEnumerator LocateResource<T>(AssetReference reference, Action<IList<IResourceLocation>> onLocated)
        {
            var locationHandle = Addressables.LoadResourceLocationsAsync(reference, typeof(T));
            yield return locationHandle;
            LoadResource(reference.AssetGUID, locationHandle, onLocated);
        }

        private void LoadResource(string key, AsyncOperationHandle<IList<IResourceLocation>> locationHandle,
            Action<IList<IResourceLocation>> onLoaded)
        {
            if (locationHandle.Status != AsyncOperationStatus.Succeeded || locationHandle.Result.Count <= 0)
            {
                LogManager.Error(nameof(AssetsService), $"加载失败: 无法找到地址键 {key}");
                return;
            }

            onLoaded?.Invoke(locationHandle.Result);
        }
    }
}