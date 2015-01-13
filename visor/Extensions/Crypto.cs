using System;
using System.IO;
using System.Security.Cryptography;

namespace Visor.Extensions
{
    public static class Crypto
    {
        private static readonly string KeyFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Visor", "Key.pkf");

        private static readonly string IVFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Visor", "IV.pkf");

        public static byte[] Encrypt(string s)
        {
            using (var rj = new RijndaelManaged())
            {
                rj.KeySize = 128;
                rj.Key = GetPrivateKey();
                rj.IV = GetPrivateIV();

                ICryptoTransform encryptor = rj.CreateEncryptor(rj.Key, rj.IV);
                byte[] encrypted;
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
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
            if (!File.Exists(KeyFilePath) || !File.Exists(IVFilePath))
            {
                throw new FileNotFoundException("Missing private key file");
            }

            using (var rj = new RijndaelManaged())
            {
                rj.KeySize = 128;
                rj.Key = GetPrivateKey();
                rj.IV = GetPrivateIV();

                ICryptoTransform decryptor = rj.CreateDecryptor(rj.Key, rj.IV);
                string decrypted;
                using (var ms = new MemoryStream(data))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            decrypted = sr.ReadToEnd();
                            sr.Close();
                        }
                    }
                }
                return decrypted;
            }
        }

        private static byte[] GetPrivateKey()
        {
            if (!File.Exists(KeyFilePath))
            {
                using (var rj = new RijndaelManaged())
                {
                    rj.KeySize = 128;
                    rj.GenerateKey();

                    string fileData = Convert.ToBase64String(rj.Key);

                    File.WriteAllText(KeyFilePath, fileData);
                }
            }

            return Convert.FromBase64String(File.ReadAllText(KeyFilePath));
        }

        private static byte[] GetPrivateIV()
        {
            if (!File.Exists(IVFilePath))
            {
                using (var rj = new RijndaelManaged())
                {
                    rj.KeySize = 128;
                    rj.GenerateIV();

                    string fileData = Convert.ToBase64String(rj.IV);

                    File.WriteAllText(IVFilePath, fileData);
                }
            }

            return Convert.FromBase64String(File.ReadAllText(IVFilePath));
        }
    }
}