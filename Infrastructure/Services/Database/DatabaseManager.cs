using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data;
using System.Data.Common;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;


namespace Infrastructure.Services.Database
{   
    public class DatabaseManager : IDatabaseManager
    {
        private readonly IConfiguration _configuration;
        private readonly ILogManager _logger;
        private readonly string _connectionString;
        private readonly DbProviderFactory _factory;

        public DatabaseManager(IConfiguration configuration, ILogManager logger)
        {
            _configuration = configuration;
            _logger = logger;

            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            _factory = DbProviderFactories.GetFactory("System.Data.SqlClient"); // SqlClient, Npgsql vs. değiştirilebilir
        }

        public async Task<List<Dictionary<string, object>>> QueryAsync(string sql, Dictionary<string, object>? parameters = null)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                using var connection = _factory.CreateConnection();
                connection!.ConnectionString = _connectionString;
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                AddParameters(command, parameters);

                using var reader = await command.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    var row = new Dictionary<string, object>();

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null! : reader.GetValue(i);
                    }

                    result.Add(row);
                }

                _logger.Info($"Query executed: {sql}");
            }
            catch (Exception ex)
            {
                _logger.Error($"Error executing query: {sql}", ex);
                throw;
            }

            return result;
        }

        public async Task<int> ExecuteAsync(string sql, Dictionary<string, object>? parameters = null)
        {
            try
            {
                using var connection = _factory.CreateConnection();
                connection!.ConnectionString = _connectionString;
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.CommandType = CommandType.Text;
                AddParameters(command, parameters);

                int affected = await command.ExecuteNonQueryAsync();
                _logger.Info($"Non-query executed: {sql} | Rows affected: {affected}");
                return affected;
            }
            catch (Exception ex)
            {
                _logger.Error($"Error executing non-query: {sql}", ex);
                throw;
            }
        }

        private void AddParameters(DbCommand command, Dictionary<string, object>? parameters)
        {
            if (parameters == null) return;

            foreach (var param in parameters)
            {
                var dbParam = command.CreateParameter();
                dbParam.ParameterName = param.Key;
                dbParam.Value = param.Value ?? DBNull.Value;
                command.Parameters.Add(dbParam);
            }
        }
    }
}

