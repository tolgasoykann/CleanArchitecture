using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Logging
{
    public abstract class BaseLogManager : ILogManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseLogManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        protected string GetTraceId()
        {
            return _httpContextAccessor.HttpContext?.Items["TraceId"]?.ToString() ?? "NoTraceId";
        }

        public abstract void Info(string message);
        public abstract void Error(string message, Exception ex = null);
        public abstract void Warning(string message);
    }
}
