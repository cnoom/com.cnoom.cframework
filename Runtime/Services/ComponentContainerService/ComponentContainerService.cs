using System.Collections.Generic;
using UnityEngine;

namespace CnoomFrameWork.Services.ComponentContainerService
{
    public class ComponentContainerService : IComponentContainerService
    {
        private static Dictionary<string, Dictionary<string, Component>> monoregisters = new Dictionary<string, Dictionary<string, Component>>();
        private static Dictionary<string,Component> globalmono = new Dictionary<string,Component>();
        #region static

        public static void RegisterMono(string key, string sceneName, Component component)
        {
            if(string.IsNullOrEmpty(sceneName))
            {
                globalmono.Add(key, component);
                return;
            }

            if(!monoregisters.ContainsKey(sceneName))
            {
                monoregisters.Add(sceneName, new Dictionary<string, Component>());
            }
            if(monoregisters[sceneName].ContainsKey(key))
            {
                //todo 提示重复注册
                monoregisters[sceneName].Remove(key);
            }
            monoregisters[sceneName].Add(key, component);
        }

        public static void UnRegisterMono(string key, string sceneName)
        {
            if(string.IsNullOrEmpty(sceneName))
            {
                globalmono.Remove(key);
                return;
            }
            if(monoregisters.ContainsKey(sceneName) && monoregisters[sceneName].ContainsKey(key))
            {
                monoregisters[sceneName].Remove(key);
            }
        }

        #endregion
        
        public void OnRegister()
        {
        }
        public void OnUnRegister()
        {
            Clear();
        }

        public void Clear(string sceneName = null)
        {
            if(string.IsNullOrEmpty(sceneName))
            {
                globalmono.Clear();
                monoregisters.Clear();
                return;
            }
            if(monoregisters.ContainsKey(sceneName))
            {
                monoregisters.Remove(sceneName);
            }
        }

        public GameObject FindGameObject(string key, string sceneName)
        {
            Component mono = FindComponent(key, sceneName);
            if(mono) return mono.gameObject;
            return null;
        }

        public T FindComponent<T>(string key, string sceneName) where T : Component
        {
            Component mono = FindComponent(key, sceneName);
            if(!mono)
            {
                return null;
            }
            if(mono is T component)
            {
                return component;
            }
            return mono.GetComponent<T>();
        }

        public T FindTransform<T>(string key, string sceneName) where T : Transform
        {
            Component mono = FindComponent(key, sceneName);
            if(mono) return mono.transform as T;
            return null;
        }

        private Component FindComponent(string key, string sceneName)
        {
            if(string.IsNullOrEmpty(sceneName))
            {
                if(globalmono.ContainsKey(key))
                {
                    return globalmono[key];
                }
                return null;
            }
            if(monoregisters.ContainsKey(sceneName) && monoregisters[sceneName].ContainsKey(key))
            {
                return monoregisters[sceneName][key];
            }
            return null;
        }

        private class MonoHolder
        {
            public string SceneName;
            public string Key;
            public MonoRegister Mono;
            public MonoHolder(string sceneName, string key, MonoRegister mono)
            {
                SceneName = sceneName;
                Key = key;
                Mono = mono;
            }
        }
    }
}