using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace FrameWork.Editor.Addressable
{
    public class AddressableProcessor
    {
        public static void SimplifyAllNames()
        {
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if(!settings)
            {
                Debug.LogError("Addressable settings not found");
                return;
            }

            Dictionary<string, string> renameMap = new Dictionary<string, string>();

            // 第一阶段：收集所有需要重命名的条目
            foreach (AddressableAssetGroup group in settings.groups)
            {
                foreach (AddressableAssetEntry entry in group.entries)
                {
                    string newAddress = AddressableNameSimplifier.SimplifyPath(entry.address);
                    renameMap[entry.address] = newAddress;
                }
            }

            // 第二阶段：应用重命名
            foreach (AddressableAssetGroup group in settings.groups)
            {
                foreach (AddressableAssetEntry entry in group.entries.ToArray()) // ToArray防止修改集合
                {
                    if(renameMap.TryGetValue(entry.address, out string newAddress))
                    {
                        entry.address = newAddress;
                    }
                }
            }

            // 第三阶段：保存修改
            AssetDatabase.SaveAssets();
            Debug.Log($"Simplified {renameMap.Count} addressable entries");
        }

        public static void ProcessAllMarkedFolders()
        {
            AddressableFolderMarker settings = AddressableFolderMarker.GetOrCreateSettings();

            foreach (string guid in settings.markedFolderGuids)
            {
                string folderPath = AssetDatabase.GUIDToAssetPath(guid);
                ProcessFolder(folderPath);
            }
        }

        private static void ProcessFolder(string folderPath)
        {
            string[] assets = AssetDatabase.FindAssets("", new[]
            {
                folderPath
            });

            foreach (string assetGuid in assets)
            {
                string path = AssetDatabase.GUIDToAssetPath(assetGuid);
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(path);

                // 设置可寻址路径
                AddressableAssetEntry entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(
                    assetGuid,
                    AddressableAssetSettingsDefaultObject.Settings.DefaultGroup);

                // 获取资源所在的文件夹名称作为标签
                string folderName = Path.GetDirectoryName(path);
                folderName = Path.GetFileName(folderName);

                // 如果标签不存在则生成该标签，并添加到资源条目
                if(!string.IsNullOrEmpty(folderName))
                {
                    List<string> labels = AddressableAssetSettingsDefaultObject.Settings.GetLabels();
                    if(!labels.Contains(folderName))
                    {
                        AddressableAssetSettingsDefaultObject.Settings.AddLabel(folderName);
                    }
                    entry.labels.Add(folderName);
                }
            }
        }
    }
}