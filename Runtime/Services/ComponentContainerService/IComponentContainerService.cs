using CnoomFrameWork.Core;
using UnityEngine;

namespace CnoomFrameWork.Services.ComponentContainerService
{
    public interface IComponentContainerService : IService
    {
        /// <param name="key"></param>
        /// <param name="sceneName"> 为空表示全局物体</param>
        public GameObject FindGameObject(string key, string sceneName = null);
        public T FindComponent<T>(string key, string sceneName = null) where T : Component;
        public T FindTransform<T>(string key, string sceneName = null) where T : Transform;
        public void Clear(string sceneName = null);
    }
}