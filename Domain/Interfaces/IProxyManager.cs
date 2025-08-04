using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IProxyManager
    {
   
       Task<TResponse?> SendAsync<TResponse>(string endpoint, HttpMethod method, object? body = null, Dictionary<string, string>? headers = null);

    }

}
