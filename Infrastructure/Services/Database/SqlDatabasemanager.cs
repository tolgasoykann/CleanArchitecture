using System.Data.SqlClient;
using Dapper;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Database
{
    public class SqlDatabaseManager : IDatabaseManager
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public SqlDatabaseManager(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("SqlClient");
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<T>(query, parameters);
        }

        public async Task<int> ExecuteAsync(string query, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(query, parameters);
        }
    }
}
