using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
                


namespace Infrastructure.Services.Logging
{
    public class FileLogManager : BaseLogManager
    {
        private readonly string _logFilePath;
        private static readonly object _lock = new object();


        public FileLogManager(IHttpContextAccessor httpContextAccessor)
            : base(httpContextAccessor)
        {
            // Logları projenin ana klasöründeki "logs" dizinine kaydediyoruz.
            var logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            _logFilePath = Path.Combine(logDir, "logs.txt");
        }
        public override void Info(string message)
        {
            lock (_lock)
            {
                File.AppendAllText(_logFilePath, $"[INFO] [TraceId: {GetTraceId()}] {message}{Environment.NewLine}");
            }
        }

        public override void Error(string message, Exception ex = null)
        {
            lock (_lock)
            {
                File.AppendAllText(_logFilePath, $"[ERROR] [TraceId: {GetTraceId()}] {message} {ex?.Message}{Environment.NewLine}");
            }
        }

        public override void Warning(string message)
        {
            lock (_lock)
            {
                File.AppendAllText(_logFilePath, $"[WARN] [TraceId: {GetTraceId()}] {message}{Environment.NewLine}");
            }
           
        }
    }
}
