using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces
{
    public interface ILogManager
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message, Exception? ex = null);
    }
}
