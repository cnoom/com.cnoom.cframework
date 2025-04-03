using System.Collections.Generic;
using UnityEngine;

namespace CnoomFrameWork.Services.ComponentContainerService
{
    public class ComponentContainerService : IComponentContainerService
    {
        private static Dictionary<string, Dictionary<string, MonoRegister>> monoregisters = new Dictionary<string, Dictionary<string, MonoRegister>>();
        private static List<MonoHolder> globalmono = new List<MonoHolder>();
        #region static

        public static void RegisterMono(string key, string sceneName, MonoRegister mono)
        {
            if(string.IsNullOrEmpty(sceneName))
            {
                globalmono.Add(new MonoHolder(sceneName, key, mono));
                return;
            }

            if(!monoregisters.ContainsKey(sceneName))
            {
                monoregisters.Add(sceneName, new Dictionary<string, MonoRegister>());
            }
            if(monoregisters[sceneName].ContainsKey(key))
            {
                //todo 提示重复注册
                monoregisters[sceneName].Remove(key);
            }
            monoregisters[sceneName].Add(key, mono);
        }

        public static void UnRegisterMono(string key, string sceneName)
        {
            if(string.IsNullOrEmpty(sceneName))
            {
                globalmono.RemoveAll(mono => mono.Key == key);
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
            MonoRegister mono = FindMono(key, sceneName);
            if(mono) return mono.gameObject;
            return null;
        }

        public T FindComponent<T>(string key, string sceneName) where T : Component
        {
            MonoRegister mono = FindMono(key, sceneName);
            if(!mono)
            {
                return null;
            }
            if(mono.component is T component)
            {
                return component;
            }
            return mono.GetComponent<T>();
        }

        public T FindTransform<T>(string key, string sceneName) where T : Transform
        {
            MonoRegister mono = FindMono(key, sceneName);
            if(mono) return mono.transform as T;
            return null;
        }

        private MonoRegister FindMono(string key, string sceneName)
        {
            if(string.IsNullOrEmpty(sceneName))
            {
                return globalmono.Find(mono => mono.Key == key).Mono;
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