using CnoomFrameWork.Modules.AddressableModule;
using CnoomFrameWork.Services.StorageService;

namespace CnoomFrameWork.Core
{
    public static class AppExtensions
    {
        /// <summary>
        ///     获取资源服务
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static AssetsService GetAssetsService(this App app)
        {
            return app.ServiceLocator.GetService<AssetsService>();
        }

        /// <summary>
        ///     获取存储服务
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IStorageService GetStorageService(this App app)
        {
            return app.ServiceLocator.GetService<IStorageService>();
        }

        /// <summary>
        ///     获取服务
        /// </summary>
        /// <returns></returns>
        public static TService GetService<TService>(this App app) where TService : class, IService
        {
            return app.ServiceLocator.GetService<TService>();
        }
    }
}