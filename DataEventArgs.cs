using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KURSOVAYA_DATABASES
{
    public class DataEventArgs : EventArgs
    {
        public bool IsSuccess { get; }
        public string Message { get; }

        public DataEventArgs(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }
    }
}
