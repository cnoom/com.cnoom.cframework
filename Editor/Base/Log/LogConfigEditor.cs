using CnoomFrameWork.Base.Log;
using UnityEditor;
using UnityEngine;

namespace CnoomFrameWork.Editor.Base.Log
{
    [CustomEditor(typeof(LogConfig))]
    public class LogConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty globalLevelThreshold;
        private SerializedProperty enableConsoleAppender;
        private SerializedProperty enableFileAppender;
        private SerializedProperty fileAppenderDirectory;
        private SerializedProperty fileAppenderFormat;
        private SerializedProperty enableAsync;
        private SerializedProperty enableArchive;

        private void OnEnable()
        {
            globalLevelThreshold = serializedObject.FindProperty("GlobalLevelThreshold");
            enableConsoleAppender = serializedObject.FindProperty("EnableConsoleAppender");
            enableFileAppender = serializedObject.FindProperty("EnableFileAppender");
            fileAppenderDirectory = serializedObject.FindProperty("FileAppenderDirectory");
            fileAppenderFormat = serializedObject.FindProperty("FileAppenderFormat");
            enableAsync = serializedObject.FindProperty("EnableAsync");
            enableArchive = serializedObject.FindProperty("EnableArchive");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("全局设置", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(globalLevelThreshold, new GUIContent("日志级别阈值", "低于此级别的日志将被忽略"));
            EditorGUILayout.PropertyField(enableAsync, new GUIContent("启用异步记录", "推荐开启以避免阻塞主线程"));
            
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("控制台输出", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableConsoleAppender, new GUIContent("启用", "是否将日志输出到Unity控制台"));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("文件输出", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(enableFileAppender, new GUIContent("启用", "是否将日志写入文件"));

            if (enableFileAppender.boolValue)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(fileAppenderDirectory, new GUIContent("日志目录", "相对于项目持久化数据目录 (Application.persistentDataPath)"));
                EditorGUILayout.PropertyField(fileAppenderFormat, new GUIContent("文件格式", "日志文件的存储格式"));
                
                EditorGUILayout.Space();
                
                EditorGUILayout.LabelField("归档设置", EditorStyles.miniBoldLabel);
                EditorGUILayout.PropertyField(enableArchive, new GUIContent("启用压缩归档", "此功能暂未实现"));
                if (enableArchive.boolValue)
                {
                    EditorGUILayout.HelpBox("日志压缩归档功能正在开发中。", MessageType.Info);
                }
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}