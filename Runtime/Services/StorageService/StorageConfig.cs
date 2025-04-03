using System;
using CnoomFrameWork.Base.Config;
using CnoomFrameWork.Core;

namespace CnoomFrameWork.Services.StorageService
{
    public class StorageConfig : IConfig
    {
        public Byte[] Key { get; private set;} = new Byte[32];
        public Byte[] Iv { get; private set;} = new Byte[16];
        public bool IsUpdateKeyIv { get; private set; } = false;

        public void SetUpdateKeyIv(bool isUpdate)
        {
            IsUpdateKeyIv = isUpdate;
        }
        
        public void SetKeyLength(int length)
        {
            Key = new Byte[length];
        }

        public void SetIvLength(int length)
        {
            Iv = new Byte[length];
        }
    }
}