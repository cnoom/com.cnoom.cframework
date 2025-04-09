using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using CnoomFrameWork.Base.Config;
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

        public void OnRegister()
        {
            storagePath = Path.Combine(Application.persistentDataPath, "Storage.json");
            LoadFromDisk();
            Application.quitting += ManualSave;
            StorageModuleHolder.Instance.SetOnApplicationPauseCallback(isPaused =>
            {
                if(!isPaused) return;
                ManualSave();
            });
        }

        public void OnUnRegister()
        {
            Application.quitting -= ManualSave;
            StorageModuleHolder.Instance.SetOnApplicationPauseCallback(null);
        }

        public void Save(string key, object value, string section = IStorageService.DefaultSection)
        {
            if(value == null) return;
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
            List<string> keyToMove = new List<string>();
            foreach (string key in storageData.dataDict.Keys)
            {
                if(key.StartsWith(prefix))
                {
                    keyToMove.Add(key);
                }
            }
            foreach (string key in keyToMove)
            {
                storageData.dataDict.Remove(key);
            }
            MakeDirty();
        }

        private void InitEncryptTool()
        {
            StorageConfig config = ConfigManager.Instance.GetConfig<StorageConfig>();
            if(PlayerPrefs.HasKey(EncryptKey) && PlayerPrefs.HasKey(EncryptIv) && !config.IsUpdateKeyIv)
            {
                byte[] key = Convert.FromBase64String(PlayerPrefs.GetString(EncryptKey));
                byte[] iv = Convert.FromBase64String(PlayerPrefs.GetString(EncryptIv));
                encryptTool = new EncryptTool(key, iv);
                return;
            }
            RandomNumberGenerator.Fill(config.Key);
            RandomNumberGenerator.Fill(config.Iv);
            PlayerPrefs.SetString(EncryptKey, Convert.ToBase64String(config.Key));
            PlayerPrefs.SetString(EncryptIv, Convert.ToBase64String(config.Iv));
            encryptTool = new EncryptTool(config.Key, config.Iv);
        }

        public void ManualSave()
        {
            if(!isDirty) return;
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
                string json = JsonUtility.ToJson(storageData);
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
                if(File.Exists(storagePath))
                {
                    string json = File.ReadAllText(storagePath);
                    json = encryptTool.Decrypt(json);
                    storageData = JsonUtility.FromJson<StorageData>(json);
                    if(storageData == null) throw new System.Exception("Invalid JSON format");
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
            public Dictionary<string, string> dataDict = new Dictionary<string, string>();

            public void Save(string key, object value)
            {
                dataDict[key] = JsonConvert.SerializeObject(value);
                #if UNITY_EDITOR
                Debug.Log("Save StorageData: " + key + " " + dataDict[key]);
                #endif
            }

            public T Load<T>(string key, T defaultValue)
            {
                if(!dataDict.TryGetValue(key, out string json)) return defaultValue;
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