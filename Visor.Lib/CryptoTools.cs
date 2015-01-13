using System;
using System.IO;
using System.Security.Cryptography;

namespace Visor.Lib
{
    public static class CryptoTools
    {
        public static byte[] Encrypt(string s, byte[] key, byte[] iv)
        {
            if (string.IsNullOrEmpty(s)) return new byte[0];
            if (key == null || key.Length == 0) throw new ArgumentException("Invalid Encryption Key", "key");
            if (iv == null || iv.Length == 0) throw new ArgumentException("Invalid Initialization Vector", "iv");

            using (var rj = new RijndaelManaged())
            {
                rj.KeySize = 128;
                rj.Key = key;
                rj.IV = iv;

                var encryptor = rj.CreateEncryptor(rj.Key, rj.IV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(s);
                            sw.Close();
                        }
                        return ms.ToArray();
                    }
                }
            }
        }

        public static string Decrypt(byte[] data, byte[] key, byte[] iv)
        {
            if (data.Length == 0) return "";
            if (key == null || key.Length == 0) throw new ArgumentException("Invalid Encryption Key", "key");
            if (iv == null || iv.Length == 0) throw new ArgumentException("Invalid Initialization Vector", "iv");

            using (var rj = new RijndaelManaged())
            {
                rj.KeySize = 128;
                rj.Key = key;
                rj.IV = iv;

                var decryptor = rj.CreateDecryptor(rj.Key, rj.IV);
                using (var ms = new MemoryStream(data))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            var result = sr.ReadToEnd();
                            sr.Close();
                            return result;
                        }
                    }
                }
            }
        }
    }
}