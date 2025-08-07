using System.Data;
using Dapper;
using Domain.Interfaces;


namespace Infrastructure.Services.Database
{
    public class DapperExecutor : IDatabaseManager
    {
        private readonly IDbConnection _connection;

        public DapperExecutor(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
        {
            return await _connection.QueryAsync<T>(query, parameters);
        }

        public async Task<int> ExecuteAsync(string query, object? parameters = null)
        {
            return await _connection.ExecuteAsync(query, parameters);
        }
    }
}
