using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace FrameWork.Editor
{
    public static class FrameWorkConfig
    {
        /// <summary>
        ///     数据路径
        /// </summary>
        private const string DataPath = "FrameWorkData";

        /// <summary>
        ///     相对数据路径
        /// </summary>
        public static string DataPathRelative
        {
            get
            {
                TryCreateFolder();
                return $"Assets/{DataPath}";
            }
        }

        /// <summary>
        ///     绝对数据路径
        /// </summary>
        public static string DataPathAbsolute
        {
            get
            {
                TryCreateFolder();
                return $"{Application.dataPath}/{DataPath}";
            }
        }
        
        /// <summary>
        /// 绝对脚本路径
        /// </summary>
        public static string ScriptPath
        {
            get
            {
                TryCreateFolder();
                return $"{DataPathAbsolute}/Scripts";
            }
        }

        [MenuItem("Test/1")]
        public static void TryCreateAsmdef()
        {
            // 检查DataPath文件夹内是否存在asmdef文件,没有就创建
            string asmdefPath = $"{DataPathRelative}/test.asmdef";
            
            if (!AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefPath))
            {
                
                ProjectWindowUtil.CreateScriptAssetFromTemplateFile($"{DataPathRelative}","test.asmdef");
                AssetDatabase.Refresh();
            }
        }

        private static void TryCreateFolder()
        {
            if(!AssetDatabase.IsValidFolder("Assets/"+DataPath))
            {
                AssetDatabase.CreateFolder("Assets", DataPath);
            }
        }
    }
}