
namespace Domain.Interfaces
{
    public interface IResiliencyService
    {
        Task<T> ExecuteWithPoliciesAsync<T>(Func<Task<T>> action, string operationKey);
    }
}
