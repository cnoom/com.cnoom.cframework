// 文件：AssetsSystem.cs

using System.Collections;
using System.Collections.Generic;
using CnoomFrameWork.Log;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace CnoomFrameWork.Modules.AddressableModule
{
    public partial class AssetsModule
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
            if(instanceMap.TryGetValue(instance, out string key))
            {
                Addressables.ReleaseInstance(instance);
                instanceMap.Remove(instance);
                ReleaseAsset(key);
            }
            else
            {
                Log.ColorLogWarningEx(nameof(AssetsModule), "尝试释放未追踪的实例对象");
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
            AsyncOperationHandle<IList<IResourceLocation>> locationHandle = Addressables.LoadResourceLocationsAsync(label);
            yield return locationHandle;

            if(locationHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Log.ColorLogErrorEx(nameof(AssetsModule), $"Failed to release assets by label: {label}");
                yield break;
            }
            foreach (IResourceLocation location in locationHandle.Result)
            {
                ReleaseAsset(location.PrimaryKey);
            }
        }

        /// <summary>
        ///     带类型检查的资源释放
        /// </summary>
        public void ReleaseAsset<T>(string key)
        {
            if(assetHandles.TryGetValue(key, out AsyncOperationHandle handle))
            {
                if(handle.Result is T)
                {
                    ReleaseAsset(key);
                }
                else
                {
                    Log.ColorLogErrorEx(nameof(AssetsModule), $"Type mismatch when releasing {key}");
                }
            }
        }

        /// <summary>
        ///     强制立即释放资源（慎用）
        /// </summary>
        public void ForceRelease(string key)
        {
            if(assetHandles.TryGetValue(key, out AsyncOperationHandle handle))
            {
                Addressables.Release(handle);
                assetHandles.Remove(key);
                referenceCount.Remove(key);
                Log.ColorLogWarningEx(nameof(AssetsModule), $"Force released: {key}");
            }
        }

        /// <summary>
        ///     释放资源
        /// </summary>
        public void ReleaseAsset(string key)
        {
            if(!referenceCount.TryGetValue(key, out int count))
            {
                Log.ColorLogWarningEx(nameof(AssetsModule), $"Attempted to release untracked asset: {key}");
                return;
            }
            count--;
            referenceCount[key] = count;

            if(count > 0) return;
            if(!assetHandles.TryGetValue(key, out AsyncOperationHandle handle)) return;
            Addressables.Release(handle);
            assetHandles.Remove(key);
            referenceCount.Remove(key);
        }

        /// <summary>
        ///     清空所有资源（慎用）
        /// </summary>
        public void ReleaseAll()
        {
            foreach (AsyncOperationHandle handle in assetHandles.Values)
                Addressables.Release(handle);

            assetHandles.Clear();
            referenceCount.Clear();
        }
    }
}