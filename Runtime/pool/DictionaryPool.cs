using System;
using System.Collections.Generic;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace pool
{
    /// <summary>
    /// 用于管理多个同类型ObjectPool的字典池 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DictionaryPool<T> where T : Object
    {
        private Dictionary<string, int> keyToIndex = new Dictionary<string, int>();
        private Dictionary<int, string> indexToKey = new Dictionary<int, string>();
        private List<ObjectPool<T>> pools = new List<ObjectPool<T>>();

        public bool ContainsKey(string key)
        {
            return keyToIndex.ContainsKey(key);
        }
        
        public void CreatePool(string key, ObjectPool<T> pool)
        {
            keyToIndex.Add(key, pools.Count);
            indexToKey.Add(pools.Count, key);
            pools.Add(pool);
        }

        public T Get(string key)
        {
            if(!keyToIndex.ContainsKey(key))
            {
                throw new NullReferenceException("poolDict not contain key:" + key);
            }
            return pools[keyToIndex[key]].Get();
        }

        public void Release(string key, T obj)
        {
            if(!keyToIndex.ContainsKey(key))
            {
                throw new NullReferenceException("poolDict not contain key:" + key);
            }
            pools[keyToIndex[key]].Release(obj);
        }

        public void RemovePool(string key)
        {
            int index = keyToIndex[key];
            keyToIndex.Remove(key);
            indexToKey.Remove(index);
            pools.RemoveAt(index);
            for(int i = index; i < pools.Count; i++)
            {
                string k = indexToKey[i + 1];
                keyToIndex[k] = i;
            }
        }

        public void ClearAll()
        {
            foreach (ObjectPool<T> pool in pools)
            {
                pool.Clear();
            }
        }
    }
}