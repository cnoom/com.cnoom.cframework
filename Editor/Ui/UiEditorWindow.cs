using System.Collections.Generic;
using CnoomFrameWork.Core;
using CnoomFrameWork.Extensions;
using CnoomFrameWork.Log;
using CnoomFrameWork.Modules.UiModule;
using CnoomFrameWork.Modules.UiModule.UiPart;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace FrameWork.Editor.Ui
{
    public class UiEditorWindow : EditorWindow
    {
        private const string UILabel = "ui";

        private void OnGUI()
        {
            UpdatePrefabDropArea();
        }

        [MenuItem("FrameWork/ui管理")]
        public static void ShowWindow()
        {
            GetWindow<UiEditorWindow>("Ui管理");
        }

        private void UpdatePrefabDropArea()
        {
            GUILayout.Label("拖入将物体加入到ui框架管理中");
            GUILayout.Label("将ui预制体拖入这里", EditorStyles.boldLabel);
            // 处理拖放操作
            Event currentEvent = Event.current;
            Rect dropArea = GUILayoutUtility.GetRect(0f, 50f, GUILayout.ExpandWidth(true));
            GUI.Box(dropArea, "拖入ui预制体区域");

            switch(currentEvent.type)
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if(!dropArea.Contains(currentEvent.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if(currentEvent.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object draggedObject in DragAndDrop.objectReferences)
                        {
                            if(draggedObject is not GameObject prefab) continue;
                            BaseUi baseUi = prefab.GetComponent<BaseUi>();
                            if(!baseUi)
                            {
                                EditorLog.Instance.ColorLogErrorEx(nameof(UiEditorWindow),$"{prefab.name}没有BaseUi组件");
                                continue;
                            }
                            TryAddPrefabToAddressable(baseUi);
                            TryAddConfigs(baseUi);
                        }
                    }
                    currentEvent.Use();
                    break;
            }
        }

        private void TryAddPrefabToAddressable(BaseUi prefab)
        {
            GameObject prefabGameObject = prefab.gameObject;
            ToAddressable(prefabGameObject);
        }

        private void TryAddConfigs(BaseUi prefab)
        {
            UiSettings uiSettings = GetUiPanels();
            if(uiSettings.panels.Contains(prefab))
            {
                return;
            }
            uiSettings.panels.Add(prefab);
            EditorUtility.SetDirty(uiSettings);
            AssetDatabase.SaveAssets();
        }

        private UiSettings GetUiPanels()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(UiSettings)}");
            if(guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                return AssetDatabase.LoadAssetAtPath<UiSettings>(path);
            }
            UiSettings uiPanels = CreateInstance<UiSettings>();
            AssetDatabase.CreateAsset(uiPanels, FrameWorkConfig.DataPathRelative + $"/{UiSettings.FileName}.asset");
            ToAddressable(uiPanels);
            return uiPanels;
        }

        private void ToAddressable(Object obj)
        {
            // 获取 Addressables 设置
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if(!settings)
            {
                EditorLog.Instance.ColorLogErrorEx(nameof(UiEditorWindow), "Addressable settings not found.");
                return;
            }

            // 获取或创建默认组
            AddressableAssetGroup defaultGroup = settings.DefaultGroup;
            if(!defaultGroup)
            {
                EditorLog.Instance.ColorLogErrorEx(nameof(UiEditorWindow), "Default Addressable group not found.");
                return;
            }

            // 获取预制体的路径
            string prefabPath = AssetDatabase.GetAssetPath(obj);

            // 检查该资源是否已经是可寻址资源
            AddressableAssetEntry existingEntry = settings.FindAssetEntry(AssetDatabase.AssetPathToGUID(prefabPath));
            if(existingEntry != null)
            {
                return;
            }

            // 创建新的可寻址资源条目
            AddressableAssetEntry newEntry = settings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID(prefabPath), defaultGroup);
            if(newEntry == null) return;
            // 设置可寻址键为预制体的名字
            newEntry.address = obj.name;
            List<string> labels = AddressableAssetSettingsDefaultObject.Settings.GetLabels();
            if(!labels.Contains(UILabel))
            {
                AddressableAssetSettingsDefaultObject.Settings.AddLabel(UILabel);
            }
            newEntry.labels.Add(UILabel);
            EditorLog.Instance.ColorLogWarningEx(nameof(UiEditorWindow), $"Added {obj.name} to Addressable with key: {obj.name}");
        }
    }
}