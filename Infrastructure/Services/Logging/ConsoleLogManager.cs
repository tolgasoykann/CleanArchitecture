using Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.Logging
{
    
    public class ConsoleLogManager : BaseLogManager
    {
        public ConsoleLogManager(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor) { }

        public override void Info(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[INFO] [TraceId: {GetTraceId()}] {message}");
            Console.ResetColor();
        }

        public override void Error(string message, Exception ex = null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] [TraceId: {GetTraceId()}] {message} {ex?.Message}");
            Console.ResetColor();
        }

        public override void Warning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN] [TraceId: {GetTraceId()}] {message}");
            Console.ResetColor();
        }
    }

}
