using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KURSOVAYA_DATABASES
{
    public class ConnectionEventArgs : EventArgs
    {
        public bool IsConnected { get; }
        public string Message { get; }

        public ConnectionEventArgs(bool isConnected, string message)
        {
            IsConnected = isConnected;
            Message = message;
        }
    }
}
