using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Infrastructure.Services.Database
{
    public class SqlDataAccess : IDataAccess
    {
        private readonly string _connectionString;

        public SqlDataAccess(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, Func<IDataReader, T> map, object? parameters = null)
        {
            var result = new List<T>();

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            AddParameters(command, parameters);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(map(reader));
            }

            return result;
        }

        public async Task<int> ExecuteNonQueryAsync(string sql, object? parameters = null)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            AddParameters(command, parameters);

            await connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        private void AddParameters(SqlCommand command, object? parameters)
        {
            if (parameters == null) return;

            foreach (var prop in parameters.GetType().GetProperties())
            {
                var value = prop.GetValue(parameters) ?? DBNull.Value;
                command.Parameters.AddWithValue("@" + prop.Name, value);
            }
        }
    }
}