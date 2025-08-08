using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Middleware
{
    public class TraceIdAwareLogManager : ILogManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TraceIdAwareLogManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetTraceId()
        {
            return _httpContextAccessor.HttpContext?.Items["TraceId"]?.ToString() ?? "NoTraceId";
        }

        public void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO] [{GetTraceId()}] {message}");
            Console.ResetColor();
        }

        public void Error(string message, Exception? ex = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] [{GetTraceId()}] {message} {ex?.Message}");
            Console.ResetColor();
        }

        public void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN] [{GetTraceId()}] {message}");
            Console.ResetColor();
        }
    }

}
