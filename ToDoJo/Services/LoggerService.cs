using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoJo.Services
{
    internal class LoggerService : ILoggerService
    {
        public void Info(string message, params object[] args) => Log.Information(message, args);
        public void Warn(string message, params object[] args) => Log.Warning(message, args);
        public void Error(Exception ex, string message = "") => Log.Error(ex, message);
        public void Error(Exception ex, string message = "", string path = null) => Log.Error(ex, message, path);
    }
}
