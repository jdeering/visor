namespace Visor.Lib
{
    public static class StringTools
    {
        private static ICryptoService _cryptoService = new EmbeddedCryptoService();

        public static byte[] Encrypt(this string s)
        {
            return _cryptoService.Encrypt(s);
        }

        public static string Decrypt(this byte[] b)
        {
            return _cryptoService.Decrypt(b);
        }
    }
}