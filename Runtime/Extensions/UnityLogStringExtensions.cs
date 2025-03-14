using UnityEngine;

namespace CnoomFrameWork.Extensions
{
    /// <summary>
    ///     Unity日志字符串扩展
    /// </summary>
    public static class UnityLogStringExtensions
    {
        public static string ColorString(this string str, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{str}</color>";
        }

        /// <summary>
        ///     消息输出
        /// </summary>
        /// <param name="tag">消息输出tag</param>
        /// <param name="message">输出的消息</param>
        /// <returns></returns>
        public static string SuffixLogMessage(this string tag, object message)
        {
            return GetStr(tag, message.ToString(), Color.white);
        }

        public static string SuffixWarningMessage(this string tag, object message)
        {
            return GetStr(tag, message.ToString(), Color.yellow);
        }

        public static string SuffixErrorMessage(this string tag, object message)
        {
            return GetStr(tag, message.ToString(), Color.red);
        }

        private static string GetStr(this string tag, string message, Color messageColor)
        {
            Color tagColor = Color.magenta;
            return $"<color=#{ColorUtility.ToHtmlStringRGB(tagColor)}>{tag}</color>: <color=#{ColorUtility.ToHtmlStringRGB(messageColor)}>{message}</color>";
        }
    }
}