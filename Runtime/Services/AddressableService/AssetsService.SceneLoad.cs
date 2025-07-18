﻿// 文件：AssetsSystem.cs

using System;
using System.Collections;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace CnoomFrameWork.Modules.AddressableModule
{
    public partial class AssetsService
    {
        /// <summary>
        ///     异步加载场景
        /// </summary>
        public void LoadScene(string sceneKey, LoadSceneMode loadMode, Action<SceneInstance> onLoaded,
            Action<string> onError = null)
        {
            app.StartCoroutine(LoadSceneCoroutine(sceneKey, loadMode, onLoaded, onError));
        }

        public IEnumerator LoadSceneCoroutine(string sceneKey, LoadSceneMode loadMode, Action<SceneInstance> onLoaded,
            Action<string> onError = null)
        {
            var operation = Addressables.LoadSceneAsync(sceneKey, loadMode);
            yield return operation;

            if (operation.Status != AsyncOperationStatus.Succeeded)
            {
                onError?.Invoke($"Failed to load scene: {sceneKey}");
                yield break;
            }

            onLoaded?.Invoke(operation.Result);
        }
    }
}