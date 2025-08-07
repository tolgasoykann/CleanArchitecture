namespace Domain.Interfaces
{
    public interface IDatabaseManager
    {
        Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null);
        Task<int> ExecuteAsync(string query, object? parameters = null);
    }   
}
