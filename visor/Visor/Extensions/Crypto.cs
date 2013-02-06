using System;
using System.IO;
using System.Security.Cryptography;

namespace Visor.Extensions
{
    public static class Crypto
    {
        private static string KeyFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Visor", "Key.pkf");
        private static string IVFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Visor", "IV.pkf");

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
            if (!File.Exists(KeyFilePath) || !File.Exists(IVFilePath))
            {
                throw new FileNotFoundException("Missing private key file");
            }

            using (RijndaelManaged rj = new RijndaelManaged())
            {
                rj.KeySize = 128;
                rj.Key = GetPrivateKey();
                rj.IV = GetPrivateIV();

                var decryptor = rj.CreateDecryptor(rj.Key, rj.IV);
                string decrypted;
                using (MemoryStream ms = new MemoryStream(data))
                {
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader sr = new StreamReader(cs))
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
                using (RijndaelManaged rj = new RijndaelManaged())
                {
                    rj.KeySize = 128;
                    rj.GenerateKey();

                    var fileData = Convert.ToBase64String(rj.Key);

                    File.WriteAllText(KeyFilePath, fileData);
                }
            }

            return Convert.FromBase64String(File.ReadAllText(KeyFilePath));
        }

        private static byte[] GetPrivateIV()
        {
            if (!File.Exists(IVFilePath))
            {
                using (RijndaelManaged rj = new RijndaelManaged())
                {
                    rj.KeySize = 128;
                    rj.GenerateIV();

                    var fileData = Convert.ToBase64String(rj.IV);

                    File.WriteAllText(IVFilePath, fileData);
                }
            }

            return Convert.FromBase64String(File.ReadAllText(IVFilePath));
        }
    }
}