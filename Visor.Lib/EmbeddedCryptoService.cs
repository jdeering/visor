using System;
using System.IO;
using System.Security.Cryptography;

namespace Visor.Lib
{
    public class EmbeddedCryptoService : ICryptoService
    {
        public string KeyFile { get; set; }
        public string IvFile { get; set; }

        public byte[] Encrypt(string s)
        {
            return CryptoTools.Encrypt(s, _key, _iv);
        }

        public string Decrypt(byte[] b)
        {
            return CryptoTools.Decrypt(b, _key, _iv);
        }

        private byte[] _key;
        protected byte[] Key
        {
            get 
            {
                if (_key == null || _key.Length == 0) GenerateKey();
                return _key;
            }
        }

        private byte[] _iv;
        private byte[] IV
        {
            get 
            {
                if (_iv == null || _iv.Length == 0) GenerateIV();
                return _iv;
            }
        }

        private void GenerateKey()
        {
            if (!File.Exists(KeyFile))
            {
                using (var rj = new RijndaelManaged())
                {
                    rj.KeySize = 128;
                    rj.GenerateKey();

                    string fileData = Convert.ToBase64String(rj.Key);

                    File.WriteAllText(KeyFile, fileData);
                }
            }

            var keyString = File.ReadAllText(KeyFile);
            _key = Convert.FromBase64String(keyString);
        }

        private void GenerateIV()
        {
            if (!File.Exists(IvFile))
            {
                using (var rj = new RijndaelManaged())
                {
                    rj.KeySize = 128;
                    rj.GenerateIV();

                    string fileData = Convert.ToBase64String(rj.IV);

                    File.WriteAllText(IvFile, fileData);
                }
            }

            var ivString = File.ReadAllText(IvFile);
            _iv = Convert.FromBase64String(ivString);
        }
    }
}