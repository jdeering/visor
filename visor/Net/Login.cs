namespace Visor.Net
{
    public class Login : ILogin
    {
        public Login(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}