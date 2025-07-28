using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;

namespace cnoom.Editor.Addressable
{
    /// <summary>
    ///     生成Addressable资源的代码
    /// </summary>
    public class AddressableCodeGenerator
    {
        private const string ClassName = "AssetsPaths";
        private const string LabelClassName = "AssetsLabels";

        public static void GenerateAddressableClass()
        {
            var addressables = GetAllAddressableAssets();
            var dic = new Dictionary<string, string>();
            var root = new DirectoryNode();

            // 构建目录树
            foreach (var entry in addressables)
            {
                // 检查是否为目录，如果是则跳过
                if (Directory.Exists(entry.Value)) continue;
                BuildDirectoryTree(root, entry.Key, entry.Value, dic);
            }

            // 扁平化目录树
            FlattenDirectoryTree(root);

            // 生成代码
            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated Addressable Hierarchy");
            sb.AppendLine("// WARNING: Do not modify manually");
            sb.AppendLine($"public static class {ClassName}");
            sb.AppendLine("{");

            GenerateNestedClasses(sb, root, 1, dic);

            sb.AppendLine("}");

            File.WriteAllText(Path.Combine(FrameWorkConfig.ScriptPath, $"{ClassName}.cs"), sb.ToString());
            AssetDatabase.Refresh();
        }

        // 递归构建目录树
        private static void BuildDirectoryTree(DirectoryNode node, string key, string value,
            Dictionary<string, string> dic)
        {
            var pathSegments = value.Split('/');
            if (pathSegments.Length == 0) return;

            for (var i = 0; i < pathSegments.Length - 1; i++)
            {
                var segment = NamingValidator.SanitizeVariableName(pathSegments[i]);
                if (!node.Children.TryGetValue(segment, out var childNode))
                {
                    childNode = new DirectoryNode();
                    node.Children[segment] = childNode;
                }

                node = childNode;
            }

            // 处理最终文件名
            var fileName = NamingValidator.SanitizeVariableName(key);
            node.Values[fileName] = key;
            dic.Add(key, value);
        }

        // 扁平化目录树，省略只有一个子目录的父目录
        private static void FlattenDirectoryTree(DirectoryNode node)
        {
            while (node.Children.Count == 1 && node.Values.Count == 0)
            {
                var singleChild = node.Children.First().Value;
                node.Children.Clear();
                foreach (var (key, value) in singleChild.Children) node.Children[key] = value;
                foreach (var (key, value) in singleChild.Values) node.Values[key] = value;
            }

            foreach (var child in node.Children.Values) FlattenDirectoryTree(child);
        }

        // 递归生成嵌套类
        private static void GenerateNestedClasses(StringBuilder sb, DirectoryNode node, int indentLevel,
            Dictionary<string, string> dic)
        {
            var indent = new string(' ', indentLevel * 4);

            // 先生成子类
            foreach (var child in node.Children)
            {
                sb.AppendLine($"{indent}public class {child.Key}");
                sb.AppendLine($"{indent}{{");
                GenerateNestedClasses(sb, child.Value, indentLevel + 1, dic);
                sb.AppendLine($"{indent}}}");
            }

            // 生成当前层的常量
            foreach (var value in node.Values)
            {
                var path = dic[value.Value];
                sb.AppendLine($"{indent}/// <summary>");
                sb.AppendLine($"{indent}/// {path}");
                sb.AppendLine($"{indent}/// </summary>");
                sb.AppendLine(
                    $"{indent}public const string {Path.GetFileNameWithoutExtension(path)} = \"{value.Value}\";");
            }
        }

        public static void GenerateLabelClass()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (!settings)
            {
                Debug.LogError("Addressable Asset Settings not found.");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated Addressable Labels");
            sb.AppendLine($"public static class {LabelClassName}");
            sb.AppendLine("{");

            // 获取所有标签
            var labels = settings.GetLabels();
            foreach (var label in labels)
            {
                var varName = NamingValidator.SanitizeVariableName(label);
                sb.AppendLine($"    public const string {varName} = \"{label}\";");
            }

            sb.AppendLine("}");

            // 写入文件
            var filePath = Path.Combine(FrameWorkConfig.ScriptPath, $"{LabelClassName}.cs");
            File.WriteAllText(filePath, sb.ToString());
            AssetDatabase.Refresh();
        }

        private static Dictionary<string, string> GetAllAddressableAssets()
        {
            var addressablePaths = new Dictionary<string, string>();

            var settings = AddressableAssetSettingsDefaultObject.Settings;
            if (!settings)
            {
                Debug.LogError("Addressable Asset Settings not found.");
                return addressablePaths;
            }

            // 遍历所有可寻址资源
            foreach (var group in settings.groups)
            {
                if (!group || group.ReadOnly)
                    continue;

                foreach (var entry in group.entries)
                {
                    if (entry == null)
                        continue;

                    var assetPath = entry.AssetPath;
                    var addressablePath = entry.address;

                    if (!addressablePaths.ContainsKey(addressablePath)) addressablePaths[addressablePath] = assetPath;
                }
            }

            return addressablePaths;
        }

        // 定义树节点类用于构建目录结构
        private class DirectoryNode
        {
            public readonly Dictionary<string, DirectoryNode> Children = new();
            public readonly Dictionary<string, string> Values = new();
        }
    }
}