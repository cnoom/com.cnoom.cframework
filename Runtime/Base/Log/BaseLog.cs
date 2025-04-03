using UnityEngine;

namespace CnoomFrameWork.Base.Log
{
    public class BaseLog : ILog
    {
        public void Log(string message, ELogType logType = ELogType.Log)
        {
            switch(logType)
            {
                case ELogType.Log:
                    Log(message, LogType.Log);
                    break;
                case ELogType.Warning:
                    Log(message, LogType.Warning);
                    break;
                case ELogType.Error:
                    Log(message, LogType.Error);
                    break;
                default:
                    Log($"Test: {message}", LogType.Log);
                    break;
            }
        }

        private void Log(string message, LogType logType)
        {
            switch(logType)
            {
                case LogType.Log:
                    Debug.Log(message);
                    break;
                case LogType.Warning:
                    Debug.LogWarning(message);
                    break;
                case LogType.Error:
                    Debug.LogError(message);
                    break;
                case LogType.Assert:
                case LogType.Exception:
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }
    }
}