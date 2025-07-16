using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;

namespace FrameWork.Editor.Addressable
{
    public class AddressableProcessor
    {
        public static void ProcessAllMarkedFolders()
        {
            var settings = AddressableFolderMarker.GetOrCreateSettings();

            foreach (var guid in settings.markedFolderGuids)
            {
                var folderPath = AssetDatabase.GUIDToAssetPath(guid);
                ProcessFolder(folderPath);
            }
        }

        private static void ProcessFolder(string folderPath)
        {
            var assets = AssetDatabase.FindAssets("", new[]
            {
                folderPath
            });

            foreach (var assetGuid in assets)
            {
                var path = AssetDatabase.GUIDToAssetPath(assetGuid);
                if (AssetDatabase.IsValidFolder(path)) continue;
                // 设置可寻址路径
                var entry = AddressableAssetSettingsDefaultObject.Settings.CreateOrMoveEntry(
                    assetGuid,
                    AddressableAssetSettingsDefaultObject.Settings.DefaultGroup);

                // 获取资源所在的文件夹名称作为标签
                var folderName = Path.GetDirectoryName(path);
                folderName = Path.GetFileName(folderName);

                // 如果标签不存在则生成该标签，并添加到资源条目
                if (!string.IsNullOrEmpty(folderName))
                {
                    var labels = AddressableAssetSettingsDefaultObject.Settings.GetLabels();
                    if (!labels.Contains(folderName))
                        AddressableAssetSettingsDefaultObject.Settings.AddLabel(folderName);
                    entry.labels.Add(folderName);
                }
            }
        }
    }
}