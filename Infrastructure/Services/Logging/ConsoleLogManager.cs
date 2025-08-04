using Domain.Interfaces;

namespace Infrastructure.Services.Logging
{
    public class ConsoleLogManager : ILogManager
    {
        public void Info(string message)
        {
            WriteLog("INFO", message, ConsoleColor.Green);
        }

        public void Warning(string message)
        {
            WriteLog("WARN", message, ConsoleColor.Yellow);
        }

        public void Error(string message, Exception? ex = null)
        {
            var fullMessage = ex == null ? message : $"{message} | Exception: {ex.Message} | StackTrace: {ex.StackTrace}";
            WriteLog("ERROR", fullMessage, ConsoleColor.Red);
        }

        private void WriteLog(string level, string message, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{level}] {message}");
            Console.ForegroundColor = originalColor;
        }
    }
}
