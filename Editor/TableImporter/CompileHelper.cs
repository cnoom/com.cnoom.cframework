using System;
using System.IO;
using UnityEditor;

namespace FrameWork.Editor.TableImporter
{
    public class CompileHelper
    {
        public static Type GetCompiledType(string className)
        {
            // 在已加载的程序集中查找该类型
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType(className);
                if (type != null)
                    return type;
            }

            UnityEngine.Debug.LogWarning($"找不到类型: {className}，请确认类名与文件路径一致并已成功编译");
            return null;
        }
    }
}