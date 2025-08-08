using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Infrastructure.Services.Logging
{
    public class CompositeLogManager : ILogManager
    {
        private readonly IEnumerable<ILogManager> _loggers;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CompositeLogManager(IEnumerable<ILogManager> loggers, IHttpContextAccessor httpContextAccessor)
        {
            _loggers = loggers;
            _httpContextAccessor = httpContextAccessor;
        }

        public void Info(string message)
        {
            foreach (var logger in _loggers)
                logger.Info(message);
        }

        public void Error(string message, Exception ex = null)
        {
            foreach (var logger in _loggers)
                logger.Error(message, ex);
        }

        public void Warning(string message)
        {
            foreach (var logger in _loggers)
                logger.Warning(message);
        }
    }
}
