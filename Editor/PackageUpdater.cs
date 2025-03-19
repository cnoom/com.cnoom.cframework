using System.Diagnostics;
using CnoomFrameWork.Log;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace FrameWork.Editor
{
    public class PackageUpdater
    {
        // 要更新的包的名称
        private const string PackageName = "com.cnoom.cframework";

        [MenuItem("FrameWork/更新框架包")]
        public static void UpdatePackage()
        {
            // 创建一个更新包的请求
            ListRequest listRequest = Client.List();
            while (!listRequest.IsCompleted)
            {
                if(listRequest.Status == StatusCode.Failure || listRequest.Error != null)
                {
                    Debug.Assert(listRequest.Error != null, "listRequest.Error != null");
                    EditorLog.Instance.ColorLogErrorEx(nameof(PackageUpdater), $"获取包列表失败: {listRequest.Error.message}");
                    UnityEngine.Debug.LogError($"获取包列表失败: {listRequest.Error.message}");
                    return;
                }
            }

            foreach (PackageInfo package in listRequest.Result)
            {
                if(package.name == PackageName)
                {
                    // 执行更新操作
                    AddRequest updateRequest = Client.Add(package.packageId);
                    while (!updateRequest.IsCompleted)
                    {
                        if(updateRequest.Status == StatusCode.Failure || updateRequest.Error != null)
                        {
                            Debug.Assert(updateRequest.Error != null, "updateRequest.Error != null");
                            UnityEngine.Debug.LogError($"更新包 {PackageName} 失败: {updateRequest.Error.message}");
                            return;
                        }
                    }

                    UnityEngine.Debug.Log($"包 {PackageName} 已成功更新。");
                    return;
                }
            }

            UnityEngine.Debug.LogError($"未找到包 {PackageName}。");
        }
    }
}