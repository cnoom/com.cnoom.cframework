using System;
using CnoomFrameWork.Core;
using CnoomFrameWork.Event;
using CnoomFrameWork.IoC;
using UnityEngine;
using UnityEngine.Scripting;

namespace Modules.ComponentContainerModule
{
    public class MonoRegister : MonoBehaviour
    {
        [Header("注册的组件名称,如果为空则使用对象名称")]
        public string key;
        [Header("注册的组件类型")]
        public Component component;
        [Inject,Preserve]
		private EventBus EventBus { get; set; }
        private bool isQuit;
        private void Awake()
        {
            App.Instance.Inject(this);
            key = string.IsNullOrEmpty(key) ? gameObject.name : key;
            EventBus.Publish(new GoContainerModule.EventRegisterComponent
            {
                Key = key,
                Component = component
            });
        }

        private void OnDestroy()
        {
            EventBus.Publish(new GoContainerModule.EventUnRegisterComponent
            {
                Key = key
            });
        }
    }
}