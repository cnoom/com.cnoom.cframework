using System;
using System.Collections.Generic;
using UnityEngine;

namespace CnoomFrameWork.Base
{
    /// <summary>
    /// 异常系统使用示例
    /// </summary>
    public class ExceptionSystemUsage : MonoBehaviour
    {
        private void Start()
        {
            // 初始化异常管理器
            // 参数1: 是否为开发模式（影响异常信息的显示方式）
            // 参数2: 异常上报接口实现（可选）
            ExceptionManager.Initialize(
                new ExceptionConfig()
                {
                    IsDevelopmentMode = true,
                    ExceptionReporter = new DefaultExceptionReporter(reportEndpoint: "https://api.example.com/report"),
                }
            );

            // 注册全局异常处理器
            ExceptionManager.RegisterHandler(new DefaultExceptionHandler());
            ExceptionManager.RegisterHandler(ExceptionHandlerFactory.CreateNetworkExceptionHandler());
            ExceptionManager.RegisterHandler(ExceptionHandlerFactory.CreateResourceExceptionHandler());
            ExceptionManager.RegisterHandler(ExceptionHandlerFactory.CreateUiExceptionHandler());

            // 注册调试变量
            ExceptionDebugHelper.RegisterGlobalVariable("GameVersion", Application.version);
            ExceptionDebugHelper.RegisterGlobalVariable("PlayerLevel", 10);
            ExceptionDebugHelper.RegisterGlobalVariable("CurrentScene", UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// 示例：安全执行代码块
        /// </summary>
        public void SafeExecute()
        {
            try
            {
                // 可能抛出异常的代码
                DoSomethingRisky();
            }
            catch (Exception ex)
            {
                // 使用异常管理器处理异常
                ExceptionManager.HandleException(ex);
                
                // 获取格式化的异常信息（根据开发/生产模式显示不同信息）
                string userMessage = ExceptionManager.GetFormattedExceptionMessage(ex);
                Debug.Log($"向用户显示: {userMessage}");
            }
        }

        /// <summary>
        /// 示例：抛出框架异常
        /// </summary>
        public void ThrowFrameworkException()
        {
            try
            {
                // 模拟资源加载失败
                throw new ResourceException(
                    ErrorCodes.ResourceNotFound,
                    "无法加载资源: player_model.prefab"
                );
            }
            catch (BaseFrameworkException ex)
            {
                // 捕获并处理框架异常
                ExceptionManager.HandleException(ex);
            }
        }

        /// <summary>
        /// 示例：异常链
        /// </summary>
        public void DemonstrateExceptionChaining()
        {
            try
            {
                try
                {
                    // 模拟网络请求失败
                    throw new System.Net.WebException("连接超时");
                }
                catch (System.Net.WebException webEx)
                {
                    // 将原始异常包装为框架异常，保留异常链
                    throw new NetworkException(
                        ErrorCodes.ConnectionFailed,
                        "无法连接到服务器",
                        webEx // 保留内部异常
                    );
                }
            }
            catch (Exception ex)
            {
                // 处理最终异常
                ExceptionManager.HandleException(ex);
                
                // 获取详细的异常信息，包括异常链
                string detailedInfo = ExceptionDebugHelper.GetDetailedExceptionInfo(ex);
                Debug.Log(detailedInfo);
            }
        }

        /// <summary>
        /// 示例：捕获变量快照
        /// </summary>
        public void CaptureVariablesExample()
        {
            try
            {
                // 模拟一些本地变量
                int playerHealth = 100;
                string currentWeapon = "Sword";
                Vector3 playerPosition = new Vector3(10, 0, 5);
                
                // 创建一个包含这些变量的对象，用于捕获快照
                var gameState = new
                {
                    Health = playerHealth,
                    Weapon = currentWeapon,
                    Position = playerPosition
                };
                
                // 模拟异常
                throw new BaseFrameworkException(
                    ErrorCodes.InvalidOperation,
                    "无效的游戏操作"
                );
            }
            catch (Exception ex)
            {
                // 捕获当前上下文的变量快照
                Dictionary<string, object> variables = ExceptionDebugHelper.CaptureVariables(this);
                
                // 创建异常上下文
                var context = new ExceptionContext
                {
                    Exception = ex,
                    CallerMember = nameof(CaptureVariablesExample),
                    CallerFile = "ExceptionSystemUsage.cs",
                    CallerLine = 123,
                    Timestamp = DateTime.UtcNow,
                    Variables = variables
                };
                
                // 手动处理异常上下文
                new DefaultExceptionHandler().HandleException(context);
            }
        }

        /// <summary>
        /// 可能抛出异常的方法
        /// </summary>
        private void DoSomethingRisky()
        {
            // 模拟随机异常
            if (UnityEngine.Random.value < 0.5f)
            {
                throw new UiException(
                    ErrorCodes.UiNotFound,
                    "无法找到UI面板: MainMenu"
                );
            }
        }
    }
}