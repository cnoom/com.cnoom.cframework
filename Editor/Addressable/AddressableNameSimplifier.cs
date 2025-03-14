using System.Collections.Generic;
using System.IO;

namespace FrameWork.Editor.Addressable
{
    /// <summary>
    ///     地址简化器
    /// </summary>
    public static class AddressableNameSimplifier
    {

        // 冲突解决计数器
        private static readonly Dictionary<string, int> _conflictTracker = new Dictionary<string, int>();

        /// <summary>
        ///     生成基于文件名的简化地址
        /// </summary>
        public static string SimplifyPath(string originalPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(originalPath);

            // 步骤1：清理文件名
            string cleanedName = NamingValidator.SanitizeVariableName(fileName);

            // 步骤2：冲突解决
            if(_conflictTracker.ContainsKey(cleanedName))
            {
                _conflictTracker[cleanedName]++;
                return $"{cleanedName}_{_conflictTracker[cleanedName]}";
            }
            _conflictTracker[cleanedName] = 0;
            return cleanedName;
        }
    }
}