using CnoomFrameWork.Base.IoC;
using CnoomFrameWork.Modules.AddressableModule;
using CnoomFrameWork.Services.ComponentContainerService;
using CnoomFrameWork.Services.StorageService;

namespace CnoomFrameWork.Core
{
    public class ServiceConfig : IocRegisterConfig
    {
        public ServiceConfig()
        {
            Register<IStorageService, StorageService>(990);
            Register<AssetsService>(970);
            Register<IComponentContainerService, ComponentContainerService>(980);
        }
    }
}