using CnoomFrameWork.Base.Log;
using UnityEngine;

namespace CnoomFrameWork.Modules.AddressableModule
{
    public partial class AssetsService
    {
        private void LogError(string message)
        {
            Log.LogWithSenderAndColor(nameof(AssetsService), message, Color.red,ELogType.Error);
        }

        public void LogWarning(string message)
        {
            Log.LogWithSenderAndColor(nameof(AssetsService), message, Color.yellow,ELogType.Warning);
        }
    }
}