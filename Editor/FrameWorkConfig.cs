using UnityEditor;
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

        private static void TryCreateFolder()
        {
            if(!AssetDatabase.IsValidFolder(DataPath))
            {
                AssetDatabase.CreateFolder("Assets", DataPath);
            }
        }
    }
}