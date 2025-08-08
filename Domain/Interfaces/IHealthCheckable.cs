
namespace Domain.Interfaces
{
    public interface IHealthCheckable
    {
        Task<bool> CheckHealthAsync();
    }
}
