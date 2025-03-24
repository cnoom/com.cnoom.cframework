using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace FrameWork.Editor
{
    public class PackageUpdater
    {
        private const string PackageName = "com.cnoom.cframework";
        private static AddRequest updateRequest;
        private static ListRequest listRequest;
        private static int progressId;

        [MenuItem("FrameWork/更新框架包")]
        public static void UpdatePackage()
        {
            if (!EditorUtility.DisplayDialog("确认更新", $"确定要更新{PackageName}吗？", "更新", "取消")) 
                return;

            progressId = Progress.Start("更新框架包");
            listRequest = Client.List();
            EditorApplication.update += OnListProgress;
        }

        private static void OnListProgress()
        {
            if (!listRequest.IsCompleted) return;
            EditorApplication.update -= OnListProgress;

            if (HandleRequestError(listRequest, "获取包列表"))
            {
                Progress.Remove(progressId);
                return;
            }

            foreach (PackageInfo package in listRequest.Result)
            {
                if (package.name == PackageName)
                {
                    updateRequest = Client.Add(package.packageId);
                    EditorApplication.update += OnUpdateProgress;
                    Progress.SetDescription(progressId, "正在下载更新...");
                    return;
                }
            }

            UnityEngine.Debug.LogError($"未找到包 {PackageName}。");
            Progress.Remove(progressId);
        }

        private static void OnUpdateProgress()
        {
            if (!updateRequest.IsCompleted) return;
            EditorApplication.update -= OnUpdateProgress;

            if (HandleRequestError(updateRequest, "更新包"))
            {
                Progress.Remove(progressId);
                return;
            }

            UnityEngine.Debug.Log($"包 {PackageName} 已成功更新。");
            Progress.Finish(progressId, Progress.Status.Succeeded);
        }

        private static bool HandleRequestError(Request request, string operation)
        {
            if (request.Status == StatusCode.Success) return false;
            
            var error = $"{(request.Error?.message ?? "未知错误")}";
            UnityEngine.Debug.LogError($"{operation}失败: {error}");
            Progress.Finish(progressId, Progress.Status.Failed);
            return true;
        }
    }
}