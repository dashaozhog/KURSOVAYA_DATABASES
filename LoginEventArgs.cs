using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KURSOVAYA_DATABASES
{
    public class LoginEventArgs : EventArgs
    {
        public bool Success { get; }
        public string Message { get; }
        public string Login { get; }

        public LoginEventArgs(bool success, string message, string login)
        {
            Success = success;
            Message = message;
            Login = login;
        }
    }
}
