using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace cnoom.Editor.Addressable
{
    /// <summary>
    ///     用于标记可寻址文件夹的脚本
    /// </summary>
    public class AddressableFolderMarker : ScriptableObject
    {
        public List<string> markedFolderGuids = new();

        [MenuItem("Assets/标记为可寻址文件夹", false, 20)]
        private static void MarkFolder()
        {
            var folder = Selection.activeObject;
            if (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(folder)))
            {
                var settings = GetOrCreateSettings();
                var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(folder));

                if (!settings.markedFolderGuids.Contains(guid))
                {
                    settings.markedFolderGuids.Add(guid);
                    EditorUtility.SetDirty(settings);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        [MenuItem("Assets/标记为可寻址文件夹", true, 21)]
        private static bool CanMarkFolder()
        {
            var folder = Selection.activeObject;
            if (!AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(folder))) return false;
            var settings = GetOrCreateSettings();
            var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(folder));
            return !settings.markedFolderGuids.Contains(guid);
        }

        public static AddressableFolderMarker GetOrCreateSettings()
        {
            // 查找现有的配置文件
            var guids = AssetDatabase.FindAssets("t:AddressableFolderMarker");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<AddressableFolderMarker>(path);
            }

            // 如果没有找到，创建一个新的配置文件
            var settings = CreateInstance<AddressableFolderMarker>();
            var assetPath = $"{FrameWorkConfig.DataPathRelative}/AssetsSettings.asset";
            AssetDatabase.CreateAsset(settings, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return settings;
        }
    }
}