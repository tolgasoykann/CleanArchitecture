using CleanArchitecture.Infrastructure.Interfaces;
using Infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infrastructure.Services.Proxy
{
    public class ProxyManager : IProxyManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogManager _logger;
        private readonly ISessionManager _sessionManager;

        public ProxyManager(HttpClient httpClient, ILogManager logger, ISessionManager sessionManager)
        {
            _httpClient = httpClient;
            _logger = logger;
            _sessionManager = sessionManager;
        }

        public async Task<TResponse?> SendAsync<TResponse>(string endpoint, HttpMethod method, object? body = null, Dictionary<string, string>? headers = null)
        {
            var request = new HttpRequestMessage(method, endpoint);

            // Header'ları ekle
            AddHeaders(request, headers);

            // Body varsa serialize et
            if (body is not null)
            {
                var json = JsonSerializer.Serialize(body);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            try
            {
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                // No content ise boş dön
                if (response.StatusCode == HttpStatusCode.NoContent)
                    return default;

                var responseJson = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<TResponse>(responseJson);
            }
            catch (Exception ex)
            {
                _logger.Error($"ProxyManager SendAsync error: {method} {endpoint}", ex);
                return default;
            }
        }

        private void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
        {
            // Varsayılan Authorization header (opsiyonel)
            var token = _sessionManager.Get<string>("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }
        }

    }

}
