using UnityEngine;

namespace CnoomFrameWork.Base.Log
{
    public class BaseLog : ILog
    {
        public bool EnableTest { get; set; } = true;
        public bool EnableLog { get; set; }  = true;
        public bool EnableWarning { get; set; } = true;
        public bool EnableError { get; set; } = true;

        internal BaseLog()
        {
            if (!Application.isEditor)
            {
                EnableTest = false;
                EnableWarning = false;
                EnableLog = false;
            }
        }

        public void Log(string message, ELogType logType = ELogType.Log, Object context = null)
        {
            switch (logType)
            {
                case ELogType.Log:
                    LogLog(message, context);
                    break;
                case ELogType.Warning:
                    LogWarning(message, context);
                    break;
                case ELogType.Error:
                    LogError(message, context);
                    break;
                case ELogType.Test:
                    LogTest(message, context);
                    break;
            }
        }

        private void LogTest(string message, Object context)
        {
            if (!EnableTest) return;
            Debug.Log(LogExtension.ColorText("[Test]:", Color.cyan) + $" {message}", context);
        }

        private void LogLog(string message, Object context)
        {
            if (!EnableLog) return;
            Debug.Log(LogExtension.ColorText("[Log]:", Color.green) + $" {message}", context);
        }

        private void LogWarning(string message, Object context)
        {
            if (!EnableWarning) return;
            Debug.LogWarning(LogExtension.ColorText("[Warning]:", Color.yellow) + $" {message}", context);
        }

        private void LogError(string message, Object context)
        {
            if (!EnableError) return;
            Debug.LogError(LogExtension.ColorText("[Error]:", Color.red) + $" {message}", context);
        }
    }
}