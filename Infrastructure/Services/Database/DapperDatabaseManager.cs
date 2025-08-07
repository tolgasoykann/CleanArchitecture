using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Database
{
    public class DapperDatabaseManager : IDatabaseManager
    {
        private readonly string _connectionString;

        public DapperDatabaseManager(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("Dapper");
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.QueryAsync<T>(query, parameters);
        }

        public async Task<int> ExecuteAsync(string query, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            return await connection.ExecuteAsync(query, parameters);
        }
    }
}
