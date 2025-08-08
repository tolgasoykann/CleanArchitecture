
namespace Domain.Interfaces
{
    public interface ILogManager
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message, Exception? ex = null);

    }
}
