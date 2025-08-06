using Domain.Interfaces;
using Polly;
using Polly.CircuitBreaker;
using Polly.RateLimit;
using Polly.Wrap;

namespace Infrastructure.Services.Resiliency
{
    public class ResiliencyService : IResiliencyService
    {
        private readonly ILogManager _logger;
        private readonly AsyncPolicyWrap _policyWrap;

        public ResiliencyService(ILogManager logger)
        {
            _logger = logger;

            var circuitBreaker = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(2, TimeSpan.FromSeconds(30),
                    onBreak: (ex, breakDelay) =>
                    {
                        _logger.Error($"Circuit opened for {breakDelay.TotalSeconds} seconds due to: {ex.Message}", ex);
                    },
                    onReset: () =>
                    {
                        _logger.Info("Circuit closed (reset).");
                    },
                    onHalfOpen: () =>
                    {
                        _logger.Info("Circuit is half-open. Trying operation...");
                    }
                );

            var rateLimiter = Policy
                .RateLimitAsync(5, TimeSpan.FromSeconds(5)); // 10 saniyede 5 istek

            _policyWrap = Policy.WrapAsync(rateLimiter, circuitBreaker);
        }

        public async Task<T> ExecuteWithPoliciesAsync<T>(Func<Task<T>> action, string operationKey)
        {
            try
            {
                return await _policyWrap.ExecuteAsync(action);
            }
            catch (RateLimitRejectedException ex)
            {
                _logger.Warning($"Rate limit exceeded for operation '{operationKey}': {ex.Message}");
                throw;
            }
            catch (BrokenCircuitException ex)
            {
                _logger.Error($"Circuit is open. Operation '{operationKey}' was not executed.", ex);
                throw;
            }
            catch (Exception ex)
            {
                _logger.Error($"Unexpected error during operation '{operationKey}'", ex);
                throw;
            }
        }
    }
}
