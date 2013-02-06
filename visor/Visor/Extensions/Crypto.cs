using System;
using System.IO;
using System.Security.Cryptography;

namespace Visor.Extensions
{
    public static class Crypto
    {
        public static byte[] Encrypt(string s)
        {
            using (RijndaelManaged rj = new RijndaelManaged())
            {
                rj.KeySize = 128;
                rj.Key = GetPrivateKey();
                rj.IV = GetPrivateIV();

                var encryptor = rj.CreateEncryptor(rj.Key, rj.IV);
                byte[] encrypted;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                        {
                            sw.Write(s);
                        }
                        encrypted = ms.ToArray();
                    }
                }

                return encrypted;
            }
        }

        public static string Decrypt(byte[] data)
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var keyFilePath = string.Join("\\", appDataFolder, "Visor", "Key.pkf");
            var ivFilePath = string.Join("\\", appDataFolder, "Visor", "IV.private");

            if (!File.Exists(keyFilePath) || !File.Exists(ivFilePath))
            {
                throw new FileNotFoundException("Missing private key file");
            }

            using (RijndaelManaged rj = new RijndaelManaged())
            {
                rj.KeySize = 128;
                rj.Key = GetPrivateKey();
                rj.IV = GetPrivateIV();

                var decryptor = rj.CreateDecryptor(rj.Key, rj.IV);
                string decrypted = "";
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    {
                        using (StreamReader sr = new StreamReader(cs))
                        {
                            decrypted = sr.ReadToEnd();
                        }
                    }
                }
                return decrypted;
            }
        }

        private static byte[] GetPrivateKey()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var keyFilePath = string.Join("\\", appDataFolder, "Visor", "Key.pkf");

            if (!File.Exists(keyFilePath))
            {
                using (RijndaelManaged rj = new RijndaelManaged())
                {
                    rj.KeySize = 128;
                    rj.GenerateKey();
                    File.WriteAllBytes(keyFilePath, rj.Key);
                }
            }

            return File.ReadAllBytes(keyFilePath);
        }

        private static byte[] GetPrivateIV()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var ivFilePath = string.Join("\\", appDataFolder, "Visor", "IV.private");
            
            if (!File.Exists(ivFilePath))
            {
                using (RijndaelManaged rj = new RijndaelManaged())
                {
                    rj.KeySize = 128;
                    rj.GenerateIV();
                    File.WriteAllBytes(ivFilePath, rj.IV);
                }
            }

            return File.ReadAllBytes(ivFilePath);
        }
    }
}