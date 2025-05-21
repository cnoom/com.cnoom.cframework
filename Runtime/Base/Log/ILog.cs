using UnityEngine;

namespace CnoomFrameWork.Base.Log
{
    public interface ILog
    {
        void Log(string message, ELogType logType = ELogType.Log);
    }

    public static class LogExtension
    {

        /// <summary>
        /// 使用发送者信息记录日志
        /// </summary>
        /// <param name="sender">日志发送者对象</param>
        /// <param name="message">日志内容</param>
        /// <param name="logType">日志类型（默认Log）</param>
        public static void LogWithSender(this ILog log, string sender, string message,ELogType logType = ELogType.Log)
        {
            log.Log(CombineMessage(sender, message),logType);
        }

        public static void LogWithColor(this ILog log, string message,Color color, ELogType logType = ELogType.Log)
        {
            log.Log(ColorText(message,color),logType);
        }

        public static void LogWithSenderAndColor(this ILog log, string sender, string message, Color color, ELogType logType = ELogType.Log)
        {
            log.Log(ColorText(CombineMessage(sender, message),color),logType);
        }
        
        public static void LogWithSenderAndColor(this ILog log, string sender, string message, Color senderColor,Color messageColor, ELogType logType = ELogType.Log)
        {
            string senderString = ColorText(sender, senderColor);
            string messageString = ColorText(message, messageColor);
            log.Log(CombineMessage(senderString, messageString),logType);
        }

        private static string CombineMessage(string sender, string message)
        {
            return $"{sender}: {message}";
        }
        
        private static string ColorText(string message,Color color)
        {
            string colorStr = ColorUtility.ToHtmlStringRGB(color);
            return $"<color=#{colorStr}>{message}</color>";
        }
    }
}