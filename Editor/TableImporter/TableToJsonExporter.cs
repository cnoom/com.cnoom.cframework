﻿using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace FrameWork.Editor.TableImporter
{
    public class TableToJsonExporter
    {
        public static void Export(string className, List<string> headers, List<string> types, List<List<string>> rows,
            string outputPath)
        {
            var type = CompileHelper.GetCompiledType(className);
            if (type == null) return;

            var list = (IList<object>)Activator.CreateInstance(typeof(List<>).MakeGenericType(type));

            foreach (var row in rows)
            {
                var obj = Activator.CreateInstance(type);
                for (int i = 0; i < headers.Count; i++)
                {
                    var field = type.GetField(headers[i]);
                    object value = ConvertHelper.ParseValue(types[i], row[i]);
                    field?.SetValue(obj, value);
                }

                list.Add(obj);
            }

            Directory.CreateDirectory(outputPath);
            var json = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(Path.Combine(outputPath, className + ".json"), json);
        }
    }
}