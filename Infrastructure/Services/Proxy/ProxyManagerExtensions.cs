using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Infrastructure.Services.Proxy
{
    public static class ProxyManagerExtensions
    {
        public static Task<T?> GetAsync<T>(this IProxyManager proxy, string url, Dictionary<string, string>? headers = null)
            => proxy.SendAsync<T>(url, HttpMethod.Get, null, headers);

        public static Task<T?> PostAsync<T>(this IProxyManager proxy, string url, object body, Dictionary<string, string>? headers = null)
            => proxy.SendAsync<T>(url, HttpMethod.Post, body, headers);

        public static Task<T?> PutAsync<T>(this IProxyManager proxy, string url, object body, Dictionary<string, string>? headers = null)
            => proxy.SendAsync<T>(url, HttpMethod.Put, body, headers);

        public static Task<T?> DeleteAsync<T>(this IProxyManager proxy, string url, Dictionary<string, string>? headers = null)
            => proxy.SendAsync<T>(url, HttpMethod.Delete, null, headers);
        public static Task<T?> PatchAsync<T>(this IProxyManager proxy, string url, object body, Dictionary<string, string>? headers = null)
            => proxy.SendAsync<T>(url, HttpMethod.Patch, body, headers);
    }
}
