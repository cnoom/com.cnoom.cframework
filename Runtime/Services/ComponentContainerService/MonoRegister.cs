using CnoomFrameWork.Base.Event;
using CnoomFrameWork.Base.IoC;
using UnityEngine;
using UnityEngine.Scripting;

namespace CnoomFrameWork.Services.ComponentContainerService
{
    public class MonoRegister : MonoBehaviour
    {
        [Header("注册的组件名称,如果为空则使用对象名称")]
        public string key;
        [Header("注册的组件类型")]
        public Component component;
        [Inject, Preserve] private EventManager EventManager { get; set; }
        private bool isQuit;
        private void Awake()
        {
            string sceneName = gameObject.scene.name;
            ComponentContainerService.RegisterMono(key, sceneName, this);
        }

        private void OnDestroy()
        {
            ComponentContainerService.UnRegisterMono(key, gameObject.scene.name);
        }
    }
}