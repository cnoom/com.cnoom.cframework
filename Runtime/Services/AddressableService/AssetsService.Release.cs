// 文件：AssetsSystem.cs

using System.Collections;
using CnoomFrameWork.Base.Log;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace CnoomFrameWork.Modules.AddressableModule
{
    public partial class AssetsService
    {
        /// <summary>
        ///     卸载场景
        /// </summary>
        public void UnloadScene(SceneInstance sceneInstance)
        {
            Addressables.UnloadSceneAsync(sceneInstance);
        }

        /// <summary>
        ///     安全释放实例化对象
        /// </summary>
        public void ReleaseInstance(GameObject instance)
        {
            if (instanceMap.TryGetValue(instance, out var key))
            {
                Addressables.ReleaseInstance(instance);
                instanceMap.Remove(instance);
                ReleaseAsset(key);
            }
            else
            {
                LogManager.Error(nameof(AssetsService), "尝试释放未追踪的实例对象");
                Object.Destroy(instance);
            }
        }

        /// <summary>
        ///     根据标签释放资源
        /// </summary>
        /// <param name="label"></param>
        public void ReleaseOfLabel(string label)
        {
            app.StartCoroutine(ReleaseOfLabelCoroutine(label));
        }

        private IEnumerator ReleaseOfLabelCoroutine(string label)
        {
            var locationHandle = Addressables.LoadResourceLocationsAsync(label);
            yield return locationHandle;

            if (locationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                LogManager.Error(nameof(AssetsService), "按标签释放资产失败: " + label);
                yield break;
            }

            foreach (var location in locationHandle.Result) ReleaseAsset(location.PrimaryKey);
        }

        /// <summary>
        ///     带类型检查的资源释放
        /// </summary>
        public void ReleaseAsset<T>(string key)
        {
            if (assetHandles.TryGetValue(key, out var handle))
            {
                if (handle.Result is T)
                    ReleaseAsset(key);
                else
                    LogManager.Error(nameof(AssetsService), $"释放时类型不匹配 {key}");
            }
        }

        /// <summary>
        ///     强制立即释放资源（慎用）
        /// </summary>
        public void ForceRelease(string key)
        {
            if (assetHandles.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                assetHandles.Remove(key);
                referenceCount.Remove(key);

                LogManager.Warn(nameof(AssetsService), $"强制释放: {key}");
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        public void ReleaseAsset(string key)
        {
            if (!referenceCount.TryGetValue(key, out var count))
            {
                LogManager.Warn(nameof(AssetsService), $"尝试释放未追踪的资产: {key}");
                return;
            }

            count--;
            referenceCount[key] = count;

            if (count > 0) return;
            if (!assetHandles.TryGetValue(key, out var handle)) return;
            Addressables.Release(handle);
            assetHandles.Remove(key);
            referenceCount.Remove(key);
        }

        /// <summary>
        ///     清空所有资源（慎用）
        /// </summary>
        public void ReleaseAll()
        {
            foreach (var handle in assetHandles.Values)
                Addressables.Release(handle);

            assetHandles.Clear();
            referenceCount.Clear();
        }
    }
}