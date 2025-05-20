using System;

namespace CnoomFrameWork.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// 检查字符串是否为 null 或空字符串
        /// </summary>
        /// <param name="str">目标字符串</param>
        /// <returns>如果是 null 或空字符串返回 true</returns>
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
    
        /// <summary>
        /// 检查字符串是否为 null、空字符串或仅包含空白字符
        /// </summary>
        /// <param name="str">目标字符串</param>
        /// <returns>如果是 null、空字符串或空白字符串返回 true</returns>
        public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);
        
        /// <summary>
        /// 使用指定参数格式化字符串
        /// </summary>
        /// <param name="format">格式字符串</param>
        /// <param name="args">格式化参数</param>
        /// <returns>格式化后的字符串</returns>
        public static string FormatWith(this string format, params object[] args) => 
            string.Format(format, args);
        
        /// <summary>
        /// 安全地获取子字符串，避免越界
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubstringSafe(this string str, int startIndex, int length)
        {
            if (str.IsNullOrEmpty()) return str;
            if (startIndex < 0) startIndex = 0;
            if (length < 0) length = 0;
            return str.Length <= startIndex ? "" 
                : str.Substring(startIndex, Math.Min(length, str.Length - startIndex));
        }
        
        /// <summary>
        /// 将字符串的首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string CapitalizeFirstLetter(this string str)
        {
            if (str.IsNullOrEmpty()) return str;
            return char.ToUpper(str[0]) + str.Substring(1).ToLower();
        }
        
        /// <summary>
        /// 将字符串转换为整数
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ToInt(this string str, int defaultValue = 0) => 
            int.TryParse(str, out int result) ? result : defaultValue;
        
        /// <summary>
        /// 移除字符串中的指定字符
        /// </summary>
        /// <param name="str"></param>
        /// <param name="toRemove"></param>
        /// <returns></returns>
        public static string Remove(this string str, string toRemove) => 
            str.Replace(toRemove, "");
        
        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Reverse(this string str)
        {
            char[] arr = str.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
        
        /// <summary>
        /// 将字符串转换为布尔值（支持 true/yes/1/on）
        /// </summary>
        /// <param name="str">目标字符串</param>
        /// <returns>转换后的布尔值，默认返回 false</returns>
        public static bool ToBool(this string str)
        {
            if (str.IsNullOrEmpty()) return false;
            return str.Trim().ToLower() switch {
                "true" => true,
                "yes" => true,
                "1" => true,
                "on" => true,
                _ => false
            };
        }
        
    }
}