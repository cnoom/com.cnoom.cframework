using System;
using CnoomFrameWork.Core;
using UnityEngine;

namespace Modules.ComponentContainerModule
{
    public class MonoRegister : MonoBehaviour
    {
        [Header("注册的组件名称,如果为空则使用对象名称")]
        public string key;
        [Header("注册的组件类型")]
        public Component component;
        private App app;
        private void Awake()
        {
            key = string.IsNullOrEmpty(key) ? gameObject.name : key;
            app.Publish(new GoContainerModule.EventRegisterComponent
            {
                Key = key,
                Component = component
            });
        }

        private void OnDestroy()
        {
            app.Publish(new GoContainerModule.EventUnRegisterComponent
            {
                Key = key
            });
        }
    }
}