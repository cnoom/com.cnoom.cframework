using UnityEditor;
using UnityEngine;

namespace FrameWork.Editor.Addressable
{
    /// <summary>
    ///     资源管理器
    /// </summary>
    public class AddressableManagerWindow : EditorWindow
    {

        private void OnGUI()
        {
            AddressableFolderMarker settings = AddressableFolderMarker.GetOrCreateSettings();

            // 显示已标记文件夹
            EditorGUILayout.LabelField("可寻址文件夹:", EditorStyles.boldLabel);

            for(var i = 0; i < settings.markedFolderGuids.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                string path = AssetDatabase.GUIDToAssetPath(settings.markedFolderGuids[i]);
                EditorGUILayout.LabelField(path);

                if(GUILayout.Button("移除", GUILayout.Width(80)))
                {
                    settings.markedFolderGuids.RemoveAt(i);
                    EditorUtility.SetDirty(settings);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }

            // 操作按钮
            if(GUILayout.Button("将可寻址文件夹内部内容标记为可循址资源"))
            {
                AddressableProcessor.ProcessAllMarkedFolders();
            }

            if(GUILayout.Button("生成可寻址资源路径类标签类"))
            {
                AddressableCodeGenerator.GenerateAddressableClass();
                AddressableCodeGenerator.GenerateLabelClass();
            }
        }
        [MenuItem("FrameWork/资源管理器")]
        public static void ShowWindow()
        {
            GetWindow<AddressableManagerWindow>("资源管理器");
        }
    }
}