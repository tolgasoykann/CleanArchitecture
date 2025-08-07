using Domain.Interfaces;

namespace Infrastructure.Services.Logging
{
    public class CompositeLogManager : ILogManager
    {
        private readonly IEnumerable<ILogManager> _loggers;

        public CompositeLogManager(IEnumerable<ILogManager> loggers)
        {
            // Composite'in kendisini dışla (sonsuz döngüye girmez)
            _loggers = loggers.Where(logger => logger is not CompositeLogManager).ToList();
        }

        public void Info(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Info(message);
            }
        }

        public void Warning(string message)
        {
            foreach (var logger in _loggers)
            {
                logger.Warning(message);
            }
        }

        public void Error(string message, Exception? ex = null)
        {
            foreach (var logger in _loggers)
            {
                logger.Error(message, ex);
            }
        }
    }
}
