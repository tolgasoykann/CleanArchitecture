using Infrastructure.Interfaces;
using System;
using System.IO;

namespace Infrastructure.Services.Logging
{
    public class FileLogManager : ILogManager
    {
        private readonly string _logFilePath;

        public FileLogManager()
        {
            var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            _logFilePath = Path.Combine(logDirectory, $"log_{DateTime.UtcNow:yyyyMMdd}.txt");
        }

        public void Info(string message)
        {
            WriteLog("INFO", message);
        }

        public void Warning(string message)
        {
            WriteLog("WARN", message);
        }

        public void Error(string message, Exception? ex = null)
        {
            var fullMessage = ex == null ? message : $"{message} | Exception: {ex.Message} | StackTrace: {ex.StackTrace}";
            WriteLog("ERROR", fullMessage);
        }

        private void WriteLog(string level, string message)
        {
            var logLine = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{level}] {message}";
            File.AppendAllText(_logFilePath, logLine + Environment.NewLine);
        }
    }
}
