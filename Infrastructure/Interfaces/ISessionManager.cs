using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Interfaces
{
    public interface ISessionManager
    {
        void Set<T>(string key, T value);
        T? Get<T>(string key);
        void Remove(string key);
        void Clear();
        string SessionId { get; }
    }
}
