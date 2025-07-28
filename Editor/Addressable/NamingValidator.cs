using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace cnoom.Editor.Addressable
{
    /// <summary>
    ///     命名验证器
    /// </summary>
    public static class NamingValidator
    {
        // C# 关键字列表（部分示例）
        private static readonly HashSet<string> _csharpKeywords = new()
        {
            // Unity 特有关键字
            "Material",
            "Shader",
            "Texture",
            "GameObject",
            "Transform",
            "RectTransform",
            "Canvas",
            "Rigidbody",
            "Collider",
            "MonoBehaviour",
            "ScriptableObject",
            "Prefab",
            "AssetBundle",
            "EditorWindow",

            // 原有 C# 关键字
            "typeof",
            "uint",
            "ulong",
            "unsafe",
            "unchecked",
            "unsafe",
            "ushort",
            "using",
            "virtual",
            "void",
            "volatile",
            "while",
            "default"
        };

        /// <summary>
        ///     生成合法的C#变量名称
        /// </summary>
        public static string SanitizeVariableName(string input)
        {
            if (string.IsNullOrEmpty(input)) return "INVALID_NAME";

            // 第一步：基础清理
            var cleanedName = new StringBuilder();
            foreach (var c in input) cleanedName.Append(char.IsLetterOrDigit(c) ? c : '_');

            // 第二步：处理特殊开头
            var sanitized = cleanedName.ToString();

            // 处理数字开头
            if (char.IsDigit(sanitized[0])) sanitized = "_" + sanitized;

            // 第三步：处理保留关键字
            if (_csharpKeywords.Contains(sanitized.ToLower())) sanitized = "@" + sanitized;

            // 第四步：去除连续下划线
            sanitized = Regex.Replace(sanitized, @"_+", "_");

            // 第五步：全大写格式
            return sanitized;
        }
    }
}