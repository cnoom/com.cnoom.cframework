using CnoomFrameWork.Core;

namespace CnoomFrameWork.Services.StorageService
{
    public interface IStorageService : IService
    {
        public const string DefaultSection = "global";
        public void Save(string key, object value, string section = DefaultSection);
        public T Get<T>(string key, T defaultValue, string section = DefaultSection);
        public void ClearSection(string section);
    }
}