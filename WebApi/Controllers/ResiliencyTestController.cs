using Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResiliencyTestController : ControllerBase
    {
        private readonly IResiliencyService _resiliencyService;
        private readonly ILogManager _logger;

        private static int _requestCount = 0;
        public ResiliencyTestController(IResiliencyService resiliencyService, ILogManager logger)
        {
            _resiliencyService = resiliencyService ?? throw new ArgumentNullException(nameof(resiliencyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("test")]
        public async Task<IActionResult> GetTest()
        {
            try
            {
                var result = await _resiliencyService.ExecuteWithPoliciesAsync(async () =>
                {
                    _requestCount++;
                    if (_requestCount > 3) // 3. isteğin sonrası hata fırlat
                        throw new Exception("Simulated failure for circuit breaker.");

                    await Task.Delay(100); // işlemi simüle et
                    _logger.Info("Servis çağrısı başarılı.");
                    return $"Başarılı sonuç {_requestCount}";
                }, "TestOperation");

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.Error("İstek sırasında hata oluştu", ex);
                return StatusCode(500, $"Hata: {ex.Message}");
            }
        }
        [HttpGet("logtest")]
        public IActionResult LogTest()
        {
            _logger.Info("Bu test logudur");
            return Ok("Log yazıldı");
        }

    }
}
