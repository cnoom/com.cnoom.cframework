using CnoomFrameWork.Base.Container;
using CnoomFrameWork.Base.Log;
using UnityEngine;
using UnityEngine.Scripting;

namespace CnoomFrameWork.Modules.AddressableModule
{
    public partial class AssetsService
    {
        [Inject] [Preserve] private ILog Log { get; set; }

        private void LogError(string message)
        {
            Log.LogWithSenderAndColor(nameof(AssetsService), message, Color.red, ELogType.Error);
        }

        public void LogWarning(string message)
        {
            Log.LogWithSenderAndColor(nameof(AssetsService), message, Color.yellow, ELogType.Warning);
        }
    }
}