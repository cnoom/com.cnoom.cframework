using System.IO;
using FrameWork.Editor;
using UnityEditor;
using UnityEngine;

namespace Editor.Base
{
    public static class CodeCreator
    {
        public static void CreateScript(string fileName, string scriptContent)
        {
            // 如果文件已存在，则提示用户
            if (File.Exists(FrameWorkConfig.ScriptPath))
            {
                Debug.LogWarning($"脚本 {fileName}.cs 已存在！将覆盖生成!");
            }

            SaveFile(fileName, scriptContent);
        }

        private static void SaveFile(string fileName, string scriptContent)
        {
            string filePath = Path.Combine(FrameWorkConfig.ScriptPath, $"{fileName}.cs");
            // 写入文件
            File.WriteAllText(filePath, scriptContent);

            // 刷新 AssetDatabase 以使 Unity 识别新文件
            AssetDatabase.Refresh();

            // 提示用户生成成功
            EditorUtility.DisplayDialog("生成成功", $"脚本 {fileName}.cs 已生成！", "确定");
        }
    }
}