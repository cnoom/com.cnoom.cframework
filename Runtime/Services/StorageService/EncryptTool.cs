namespace CnoomFrameWork.Services.StorageService
{
    public class EncryptTool
    {
        private readonly byte[] _key;
        private readonly byte[] _iv;

        // 构造函数（支持自定义密钥和IV）
        public EncryptTool(byte[] key, byte[] iv)
        {
            _key = key;
            _iv = iv;
        }

        /// <summary>
        /// 加密
        /// </summary>
        public string Encrypt(string encryptStr)
        {
            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

                var encryptor = aes.CreateEncryptor();
                var plainBytes = System.Text.Encoding.UTF8.GetBytes(encryptStr);

                using (var ms = new System.IO.MemoryStream())
                using (var cs = new System.Security.Cryptography.CryptoStream(ms, encryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                    return System.Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        /// <summary>
        /// 解密
        /// </summary>
        public string Decrypt(string decryptStr)
        {
            using (var aes = System.Security.Cryptography.Aes.Create())
            {
                aes.Key = _key;
                aes.IV = _iv;
                aes.Mode = System.Security.Cryptography.CipherMode.CBC;
                aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

                var decryptor = aes.CreateDecryptor();
                var cipherBytes = System.Convert.FromBase64String(decryptStr);

                using (var ms = new System.IO.MemoryStream())
                using (var cs = new System.Security.Cryptography.CryptoStream(ms, decryptor, System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    cs.Write(cipherBytes, 0, cipherBytes.Length);
                    cs.FlushFinalBlock();
                    return System.Text.Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }
}