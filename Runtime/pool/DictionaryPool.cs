using System;
using System.Collections.Generic;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

namespace pool
{
    /// <summary>
    ///     用于管理多个同类型ObjectPool的字典池
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DictionaryPool<T> where T : Object
    {
        private readonly Dictionary<int, string> indexToKey = new();
        private readonly Dictionary<string, int> keyToIndex = new();
        private readonly List<ObjectPool<T>> pools = new();

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
            if (!keyToIndex.ContainsKey(key)) throw new NullReferenceException("poolDict not contain key:" + key);
            return pools[keyToIndex[key]].Get();
        }

        public void Release(string key, T obj)
        {
            if (!keyToIndex.ContainsKey(key)) throw new NullReferenceException("poolDict not contain key:" + key);
            pools[keyToIndex[key]].Release(obj);
        }

        public void RemovePool(string key)
        {
            var index = keyToIndex[key];
            keyToIndex.Remove(key);
            indexToKey.Remove(index);
            pools.RemoveAt(index);
            for (var i = index; i < pools.Count; i++)
            {
                var k = indexToKey[i + 1];
                keyToIndex[k] = i;
            }
        }

        public void ClearAll()
        {
            foreach (var pool in pools) pool.Clear();
        }
    }
}