using UnityEngine;

namespace CnoomFrameWork.Base.Log
{
    public interface ILog
    {
        void Log(string message, ELogType logType);
    }

    public static class LogExtension
    {

        public static void LogTestEx(this ILog log, string message)
        {
            log.Log(message, ELogType.Test);
        }

        public static void LogEx(this ILog log, string message)
        {
            log.Log(message, ELogType.Log);
        }

        public static void LogErrorEx(this ILog log, string message)
        {
            log.Log(message, ELogType.Error);
        }

        public static void LogWarningEx(this ILog log, string message)
        {
            log.Log(message, ELogType.Warning);
        }

        public static void ColorLogEx(this ILog log, string sender, string message)
        {
            log.Log(ColorText(sender, message, Color.green, Color.white), ELogType.Log);
        }

        public static void ColorLogErrorEx(this ILog log, string sender, string message)
        {
            log.Log(ColorText(sender, message, Color.green, Color.red), ELogType.Error);
        }

        public static void ColorLogWarningEx(this ILog log, string sender, string message)
        {
            log.Log(ColorText(sender, message, Color.green, Color.yellow), ELogType.Warning);
        }

        public static void ColorLogTestEx(this ILog log, string sender, string message)
        {
            log.Log(ColorText(sender, message, Color.green, Color.cyan), ELogType.Test);
        }

        private static string ColorText(string sender, string message, Color senderColor, Color messageColor)
        {
            string senderColorStr = ColorUtility.ToHtmlStringRGB(senderColor);
            string messageColorStr = ColorUtility.ToHtmlStringRGB(messageColor);
            return $"<color=#{senderColorStr}>{sender}</color>: <color=#{messageColorStr}>{message}</color>";
        }
    }
}