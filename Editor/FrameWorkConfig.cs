using UnityEditor;
using UnityEditor.Search;
using UnityEditorInternal;
using UnityEngine;

namespace FrameWork.Editor
{
    public static class FrameWorkConfig
    {
        /// <summary>
        ///     数据文件名
        /// </summary>
        private const string DataFolderName = "FrameWorkData";

        /// <summary>
        ///     相对数据路径
        /// </summary>
        public static string DataPathRelative
        {
            get
            {
                TryCreateFolder("Assets", DataFolderName);
                return $"Assets/{DataFolderName}";
            }
        }

        /// <summary>
        ///     绝对数据路径
        /// </summary>
        public static string DataPathAbsolute
        {
            get
            {
                TryCreateFolder("Assets", DataFolderName);
                return $"{Application.dataPath}/{DataFolderName}";
            }
        }

        /// <summary>
        /// 绝对脚本路径
        /// </summary>
        public static string ScriptPath
        {
            get
            {
                string path = DataPathAbsolute+"/Scripts";
                TryCreateFolder("Assets/" + DataFolderName, "Scripts");
                return path;
            }
        }

        private static void TryCreateFolder(string path, string folderName)
        {
            if(!AssetDatabase.IsValidFolder(path + "/" + folderName))
            {
                AssetDatabase.CreateFolder(path, folderName);
            }
        }
    }
}