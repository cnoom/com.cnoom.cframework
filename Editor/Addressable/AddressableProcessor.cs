using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace FrameWork.Editor.Addressable
{
    public class AddressableProcessor
    {
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
                if(AssetDatabase.IsValidFolder(path))
                {
                    continue;
                }
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