using UnityEngine;

namespace CnoomFrameWork.Base.Log
{
    public interface ILog
    {
        bool EnableTest { get; set; }
        bool EnableLog { get; set; }
        bool EnableWarning { get; set; }
        bool EnableError { get; set; }

        void Log(string message, ELogType logType = ELogType.Log, Object context = null);
    }

    public static class LogExtension
    {
        /// <summary>
        ///     使用发送者信息记录日志
        /// </summary>
        /// <param name="sender">日志发送者对象</param>
        /// <param name="message">日志内容</param>
        /// <param name="logType">日志类型（默认Log）</param>
        /// <param name="context">关联的物体</param>
        public static void LogWithSender(this ILog log, string sender, string message, ELogType logType = ELogType.Log,
            Object context = null)
        {
            log.Log(CombineMessage(sender, message), logType, context);
        }

        public static void LogWithColor(this ILog log, string message, Color color, ELogType logType = ELogType.Log,
            Object context = null)
        {
            log.Log(ColorText(message, color), logType, context);
        }

        public static void LogWithSenderAndColor(this ILog log, string sender, string message, Color color,
            ELogType logType = ELogType.Log, Object context = null)
        {
            log.Log(ColorText(CombineMessage(sender, message), color), logType, context);
        }

        public static void LogWithSenderAndColor(this ILog log, string sender, string message, Color senderColor,
            Color messageColor, ELogType logType = ELogType.Log, Object context = null)
        {
            var senderString = ColorText(sender, senderColor);
            var messageString = ColorText(message, messageColor);
            log.Log(CombineMessage(senderString, messageString), logType, context);
        }

        private static string CombineMessage(string sender, string message)
        {
            return $"{sender}: {message}";
        }

        internal static string ColorText(string message, Color color)
        {
            var colorStr = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{colorStr}>{message}</color>";
        }
    }
}