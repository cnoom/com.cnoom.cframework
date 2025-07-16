using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CnoomFrameWork.Services.StorageService
{
    public class EncryptTool
    {
        private readonly byte[] _iv;
        private readonly byte[] _key;

        // 构造函数（支持自定义密钥和IV）
        public EncryptTool(byte[] key, byte[] iv)
        {
            _key = key;
            _iv = iv;
        }

        /// <summary>
        ///     加密
        /// </summary>
        public string Encrypt(string encryptStr)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var encryptor = aes.CreateEncryptor();
                var plainBytes = Encoding.UTF8.GetBytes(encryptStr);

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        ///     解密
        /// </summary>
        public string Decrypt(string decryptStr)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                var decryptor = aes.CreateDecryptor();
                var cipherBytes = Convert.FromBase64String(decryptStr);

                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.FlushFinalBlock();
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }
}