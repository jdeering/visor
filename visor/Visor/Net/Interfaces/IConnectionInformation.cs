namespace Visor.Net
{
    public interface IConnectionInformation
    {
        string Server { get; set; }
        int Port { get; set; }
    }
}