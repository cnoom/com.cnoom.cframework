﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Core;
using CnoomFrameWork.Singleton;
using Newtonsoft.Json;
using UnityEngine;

namespace CnoomFrameWork.Services.StorageService
{
    public class StorageService : IStorageService
    {
        private const string EncryptKey = "encryptkey";
        private const string EncryptIv = "encryptiv";
        private EncryptTool encryptTool;
        private bool isDirty;
        private StorageData storageData;
        private StorageModuleHolder storageGameObject;
        private string storagePath;

        void IService.Initialize()
        {
            storagePath = Path.Combine(Application.persistentDataPath, "Storage.json");
            InitEncryptTool();
            LoadFromDisk();
            Application.quitting += ManualSave;
            StorageModuleHolder.Instance.SetOnApplicationPauseCallback(isPaused =>
            {
                if (!isPaused) return;
                ManualSave();
            });
        }

        public void Dispose()
        {
            Application.quitting -= ManualSave;
            StorageModuleHolder.Instance.SetOnApplicationPauseCallback(null);
        }

        public void Save(string key, object value, string section = IStorageService.DefaultSection)
        {
            if (value == null) return;
            key = $"{section}.{key}";
            storageData.Save(key, value);
            MakeDirty();
        }

        public T Get<T>(string key, T defaultValue, string section = IStorageService.DefaultSection)
        {
            key = $"{section}.{key}";
            return storageData.Load(key, defaultValue);
        }

        public void ClearSection(string section)
        {
            var prefix = $"{section}.";
            var keyToMove = new List<string>();
            foreach (var key in storageData.dataDict.Keys)
                if (key.StartsWith(prefix))
                    keyToMove.Add(key);

            foreach (var key in keyToMove) storageData.dataDict.Remove(key);

            MakeDirty();
        }

        private void InitEncryptTool()
        {
            var config = ConfigManager.Instance.GetConfig<StorageConfig>();
            if (PlayerPrefs.HasKey(EncryptKey) && PlayerPrefs.HasKey(EncryptIv))
            {
                var key = Convert.FromBase64String(PlayerPrefs.GetString(EncryptKey));
                var iv = Convert.FromBase64String(PlayerPrefs.GetString(EncryptIv));
                if (key.Length == config.Key.Length && iv.Length == config.Iv.Length)
                {
                    encryptTool = new EncryptTool(key, iv);
                    return;
                }
            }

            RandomNumberGenerator.Fill(config.Key);
            RandomNumberGenerator.Fill(config.Iv);
            PlayerPrefs.SetString(EncryptKey, Convert.ToBase64String(config.Key));
            PlayerPrefs.SetString(EncryptIv, Convert.ToBase64String(config.Iv));
            encryptTool = new EncryptTool(config.Key, config.Iv);
        }

        public void ManualSave()
        {
            if (!isDirty) return;
            SaveToDisk();
        }

        private void MakeDirty()
        {
            isDirty = true;
        }

        // 磁盘持久化
        private void SaveToDisk()
        {
            try
            {
                var json = JsonConvert.SerializeObject(storageData);
                json = encryptTool.Encrypt(json);
                File.WriteAllText(storagePath, json);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Storage save failed: {e.Message}");
            }
        }


        // 反持久化
        private void LoadFromDisk()
        {
            try
            {
                if (File.Exists(storagePath))
                {
                    var json = File.ReadAllText(storagePath);
                    json = encryptTool.Decrypt(json);
                    storageData = JsonConvert.DeserializeObject<StorageData>(json);
                    if (storageData == null) throw new System.Exception("Invalid JSON format");
                }
                else
                {
                    storageData = new StorageData();
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"Storage load failed: {e.Message}");
                storageData = new StorageData();
            }
        }

        // 存储数据结构
        [Serializable]
        private class StorageData
        {
            public Dictionary<string, string> dataDict = new();

            public void Save(string key, object value)
            {
                dataDict[key] = JsonConvert.SerializeObject(value);
#if UNITY_EDITOR
                Debug.Log("Save StorageData: " + key + " " + dataDict[key]);
#endif
            }

            public T Load<T>(string key, T defaultValue)
            {
                if (!dataDict.TryGetValue(key, out var json)) return defaultValue;
                try
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
                catch (System.Exception e)
                {
#if UNITY_EDITOR
                    Debug.LogError("Load StorageData failed: " + e.Message);
                    Debug.LogError("Key: " + key + " Value: " + json);
#endif
                    return defaultValue;
                }
            }
        }

        private class StorageModuleHolder : PersistentMonoSingleton<StorageModuleHolder>
        {
            private Action<bool> onApplicationPauseCallback;

            private void OnApplicationPause(bool pauseStatus)
            {
                onApplicationPauseCallback?.Invoke(pauseStatus);
            }

            public void SetOnApplicationPauseCallback(Action<bool> callback)
            {
                onApplicationPauseCallback = callback;
            }
        }
    }
}