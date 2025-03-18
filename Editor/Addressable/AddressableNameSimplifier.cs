using System.Collections.Generic;
using System.IO;

namespace FrameWork.Editor.Addressable
{
    /// <summary>
    ///     地址简化器
    /// </summary>
    public static class AddressableNameSimplifier
    {

     

        /// <summary>
        ///     生成基于文件名的简化地址
        /// </summary>
        public static string SimplifyPath(string originalPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(originalPath);

            // 步骤1：清理文件名
            string cleanedName = NamingValidator.SanitizeVariableName(fileName);
            
            return cleanedName;
        }
    }
}