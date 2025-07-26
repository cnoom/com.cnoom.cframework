using System;
using System.Collections.Generic;

namespace FrameWork.Editor.TableImporter
{
    public class ConvertHelper
    {
        public static object ParseValue(string type, string value)
        {
            try
            {
                switch (type)
                {
                    case "int": return int.Parse(value);
                    case "float": return float.Parse(value);
                    case "string": return value;
                    case "bool": return bool.Parse(value);
                    case "List<int>": return new List<int>(Array.ConvertAll(value.Split(';'), int.Parse));
                    default: return null;
                }
            }
            catch
            {
                UnityEngine.Debug.LogWarning($"类型解析失败: {type} 值: {value}");
                return null;
            }
        }
    }
}