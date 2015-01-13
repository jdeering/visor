namespace Visor.Lib
{
    public interface ICryptoService
    {
        byte[] Encrypt(string s);
        string Decrypt(byte[] b);
    }
}