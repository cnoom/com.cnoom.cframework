using CnoomFrameWork.Core;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using CnoomFrameWork.Singleton;
using Newtonsoft.Json;

namespace Modules.StorageModule
{
    public class StorageModule : Module
    {
        private const string DefaultSection = "global";
        private const string EncryptKey = "encryptkey";
        private const string EncryptIv = "encryptiv";
        private string storagePath;
        private StorageData storageData;
        private StorageModuleHolder storageGameObject;
        private EncryptTool encryptTool;
        private bool isDirty;

        protected override void OnInitialize()
        {
            storagePath = Path.Combine(Application.persistentDataPath, "GameStorage.json");
            LoadFromDisk();
            Application.quitting += ManualSave;
            StorageModuleHolder.Instance.SetOnApplicationPauseCallback(isPaused =>
            {
                if(!isPaused) return;
                ManualSave();
            });
        }

        private void InitEncryptTool()
        {
            StorageConfig config = ConfigManager.Instance.GetConfig<StorageConfig>();
            if(PlayerPrefs.HasKey(EncryptKey) && PlayerPrefs.HasKey(EncryptIv) && !config.IsUpdateKeyIv)
            {
                var key = Convert.FromBase64String(PlayerPrefs.GetString(EncryptKey));
                var iv = Convert.FromBase64String(PlayerPrefs.GetString(EncryptIv));
                encryptTool = new EncryptTool(key, iv);
                return;
            }
            RandomNumberGenerator.Fill(config.Key);
            RandomNumberGenerator.Fill(config.Iv);
            PlayerPrefs.SetString(EncryptKey, Convert.ToBase64String(config.Key));
            PlayerPrefs.SetString(EncryptIv, Convert.ToBase64String(config.Iv));
            encryptTool = new EncryptTool(config.Key, config.Iv);
        }

        public override void Dispose()
        {
            base.Dispose();
            Application.quitting -= ManualSave;
        }

        public void BatchSave(string section = DefaultSection, params (string key, object value)[] dataList)
        {
            foreach ((string key, object value) valueTuple in dataList)
            {
                Save(valueTuple.key, valueTuple.value, section);
            }
        }

        // 保存数据到内存
        public void Save(string key, object value, string section = DefaultSection)
        {
            storageData.Save($"{section}.{key}", value);
            MakeDirty();
        }

        // 加载数据
        public T Load<T>(string key, T defaultValue = default, string section = DefaultSection)
        {
            return storageData.Load($"{section}.{key}", defaultValue);
        }

        public void ManualSave()
        {
            if(!isDirty) return;
            SaveToDisk();
        }

        public void ClearSection(string section)
        {
            var prefix = $"{section}.";
            var keyToMove = new List<string>();
            foreach (var key in storageData.dataDict.Keys)
            {
                if(key.StartsWith(prefix))
                {
                    keyToMove.Add(key);
                }
            }
            foreach (var key in keyToMove)
            {
                storageData.dataDict.Remove(key);
            }
            MakeDirty();
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
            catch (Exception e)
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
                    if(storageData == null) throw new Exception("Invalid JSON format");
                }
                else
                {
                    storageData = new StorageData();
                }
            }
            catch (Exception e)
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
                catch (Exception e)
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

            public void SetOnApplicationPauseCallback(Action<bool> callback)
            {
                onApplicationPauseCallback = callback;
            }

            private void OnApplicationPause(bool pauseStatus)
            {
                onApplicationPauseCallback?.Invoke(pauseStatus);
            }
        }
    }
}