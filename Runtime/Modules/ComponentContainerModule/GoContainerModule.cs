using System;
using System.Collections.Generic;
using CnoomFrameWork.Core;
using CnoomFrameWork.Event;
using UnityEngine;
using UnityEngine.Scripting;

namespace Modules.ComponentContainerModule
{
    public class GoContainerModule : Module
    {
        private Dictionary<string, Component> components = new Dictionary<string, Component>();

        public bool HasComponent(string key) => components.ContainsKey(key);

        public T GetComponent<T>(string key) where T : Component => components[key] is T ? (T)components[key] : GetGameObject(key).GetComponent<T>();

        public GameObject GetGameObject(string key) => components[key].gameObject;
        
        public void RemoveComponent(string key) => components.Remove(key);

        public void RegisterComponent(string key, Component component)
        {
            if(components.ContainsKey(key))
            {
                throw new Exception($"同名 {key} 已经存在,为 {components[key].GetType()}类型");
            }
            components.Add(key, component);
        }

        [Subscribe, Preserve]
        private void OnRegisterComponent(EventRegisterComponent e)
        {
            RegisterComponent(e.Key, e.Component);
        }

        [Subscribe, Preserve]
        private void OnUnRegisterComponent(EventUnRegisterComponent e)
        {
            components.Remove(e.Key);
        }

        public struct EventRegisterComponent
        {
            public string Key;
            public Component Component;
        }

        public struct EventUnRegisterComponent
        {
            public string Key;
        }
    }
}