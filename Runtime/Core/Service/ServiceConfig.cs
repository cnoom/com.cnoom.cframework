using CnoomFrameWork.Base.Container;
using CnoomFrameWork.Modules.AddressableModule;
using CnoomFrameWork.Services.StorageService;
using CnoomFrameWork.Services.TimerService;

namespace CnoomFrameWork.Core
{
    public class ServiceConfig : IocRegisterConfig
    {
        public ServiceConfig()
        {
            Register<IStorageService, StorageService>(990);
            Register<AssetsService>(950);
            Register<TimerService>(900);
        }
    }
}