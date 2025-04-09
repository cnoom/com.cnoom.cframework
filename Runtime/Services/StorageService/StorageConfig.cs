using CnoomFrameWork.Base.Config;

namespace CnoomFrameWork.Services.StorageService
{
    public class StorageConfig : IConfig
    {
        public byte[] Key { get; private set; } = new byte[32];
        public byte[] Iv { get; private set; } = new byte[16];
        public bool IsUpdateKeyIv { get; private set; }

        public void SetUpdateKeyIv(bool isUpdate)
        {
            IsUpdateKeyIv = isUpdate;
        }

        public void SetKeyLength(int length)
        {
            Key = new byte[length];
        }

        public void SetIvLength(int length)
        {
            Iv = new byte[length];
        }
    }
}