using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;

namespace FrameWork.Editor.TableImporter
{
    public class TableToClassGenerator
    {
        public static void Generate(string className, List<string> headers, List<string> types, string outputPath)
        {
            var sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine($"public class {className}");
            sb.AppendLine("{");

            for (int i = 0; i < headers.Count; i++)
                sb.AppendLine($"    public {types[i]} {headers[i]};");

            sb.AppendLine("}");
            Directory.CreateDirectory(outputPath);
            File.WriteAllText(Path.Combine(outputPath, className + ".cs"), sb.ToString());
            AssetDatabase.Refresh();
        }
    }
}