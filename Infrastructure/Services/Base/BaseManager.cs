using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Base
{
    public abstract class BaseManager : ITraceable, IHealthCheckable
    {
        protected readonly ILogManager _logManager;
        public string TraceId { get; set; } = Guid.NewGuid().ToString();

        protected BaseManager(ILogManager logManager)
        {
            _logManager = logManager;
        }

        protected void Log(string message)
        {
            _logManager.Info($"[{TraceId}] {message}");
        }

        protected void LogError(string message)
        {
            _logManager.Error($"[{TraceId}] {message}");
        }

        public abstract Task<bool> CheckHealthAsync();
    }

}
