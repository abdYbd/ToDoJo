using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoJo.Services
{
    public interface ILoggerService
    {
        void Info(string message, params object[] args);
        void Warn(string message, params object[] args);
        void Error(Exception ex, string message = "");
        void Error(Exception ex, string message = "", string path = null);

    }
}
