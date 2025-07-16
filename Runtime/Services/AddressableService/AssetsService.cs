using System;
using System.Collections.Generic;
using CnoomFrameWork.Core;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CnoomFrameWork.Modules.AddressableModule
{
    public partial class AssetsService : AService
    {
        private readonly App app = App.Instance;

        // 资源追踪数据结构
        private readonly Dictionary<string, AsyncOperationHandle> assetHandles = new();
        private readonly Dictionary<GameObject, string> instanceMap = new();
        private readonly Dictionary<string, AsyncOperationHandle> loadingHandles = new();
        private readonly Dictionary<string, int> referenceCount = new();

        public override void Dispose()
        {
            foreach (var asyncOperationHandle in assetHandles) asyncOperationHandle.Value.Release();
            assetHandles.Clear();
            foreach (var gameObject in instanceMap.Keys) GameObject.Destroy(gameObject);
            instanceMap.Clear();
            foreach (var asyncOperationHandle in loadingHandles) asyncOperationHandle.Value.Release();
            loadingHandles.Clear();
            referenceCount.Clear();
        }

        #region 内部辅助组件

        // 实例释放追踪器
        private class InstanceReleaseTracker : MonoBehaviour
        {
            private Action onDestroyAction;

            private void OnDestroy()
            {
                onDestroyAction?.Invoke();
            }

            public void Init(Action onDestroy)
            {
                onDestroyAction = onDestroy;
            }
        }

        #endregion

        #region 增强型追踪方法（关键实现）

        private void TrackHandle(string key, AsyncOperationHandle handle)
        {
            // 资源已加载的情况
            if (!assetHandles.TryAdd(key, handle))
            {
                referenceCount[key]++;
                return;
            }

            // 新资源加载的情况
            referenceCount.Add(key, 1);

            // 自动释放监听
            handle.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded) return;
                assetHandles.Remove(key);
                referenceCount.Remove(key);
            };
        }


        // 实例化对象追踪
        private void TrackInstance(string key, GameObject instance)
        {
            instanceMap[instance] = key;

            // 自动释放监听
            var tracker = instance.AddComponent<InstanceReleaseTracker>();
            tracker.Init(() => ReleaseInstance(instance));
        }

        #endregion
    }
}