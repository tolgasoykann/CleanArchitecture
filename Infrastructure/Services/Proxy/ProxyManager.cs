using CleanArchitecture.Domain.Interfaces;
using Domain.Interfaces;
using System.Net.Http.Headers;
using System.Net;
using System.Text;

namespace Infrastructure.Services.Proxy
{
    public class ProxyManager : IProxyManager
    {
        private readonly HttpClient _httpClient;
        private readonly ILogManager _logger;
        private readonly ISessionManager _sessionManager;
        private readonly ICustomJsonSerializer _jsonSerializer;

        public ProxyManager(HttpClient httpClient, ILogManager logger, ISessionManager sessionManager, ICustomJsonSerializer jsonSerializer)
        {
            _httpClient = httpClient;
            _logger = logger;
            _sessionManager = sessionManager;
            _jsonSerializer = jsonSerializer;
        }

        public async Task<TResponse?> SendAsync<TResponse>(string endpoint, HttpMethod method, object? body = null, Dictionary<string, string>? headers = null)
        {
            var request = new HttpRequestMessage(method, endpoint);
            _logger.Info($"Proxy isteği hazırlanıyor: {method} {endpoint}");
            AddHeaders(request, headers);

            if (body is not null)
            {
                var json = _jsonSerializer.Serialize(body);
                _logger.Info($"Proxy body: {json}");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");
            }

            try
            {
                using var response = await _httpClient.SendAsync(request);
                _logger.Info($"Proxy cevabı alındı: {response.StatusCode}");
                response.EnsureSuccessStatusCode();

                if (response.StatusCode == HttpStatusCode.NoContent)
                    return default;

                var responseJson = await response.Content.ReadAsStringAsync();
                _logger.Info($"Proxy response body: {responseJson}");
                return _jsonSerializer.Deserialize<TResponse>(responseJson);
            }
            catch (Exception ex)
            {
                _logger.Error($"ProxyManager SendAsync hata: {method} {endpoint}", ex);
                return default;
            }
        }

        private void AddHeaders(HttpRequestMessage request, Dictionary<string, string>? headers)
        {
            var token = _sessionManager.Get<string>("AccessToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                _logger.Info("Bearer token header eklendi.");
            }

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    _logger.Info($"Header eklendi: {header.Key}={header.Value}");
                }
            }
        }
        public async Task<T?> GetAsync<T>(string endpoint)
        {
            return await SendAsync<T>(endpoint, HttpMethod.Get);
        }


    }

}
