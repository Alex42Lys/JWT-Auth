namespace WebApplication4
{
    public class LoginData
    {

        private string? _password;
        private string? _login;
        private int id;
        public LoginData(string login, string password) 
        {
            _login = login;
            _password = password;
        }
        public LoginData() { }


        public string Login { get { return _login; } set { _login = value; } }

        public string Password { get { return _password; } set { _password = value; } }

        public int ID
        {
            get { return id; }
            set { id = value; }
        }
    }
}
