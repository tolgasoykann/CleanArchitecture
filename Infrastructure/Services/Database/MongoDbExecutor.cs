using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Database
{
    public class MongoDbExecutor : IDatabaseManager
    {
        public async Task<IEnumerable<T>> QueryAsync<T>(string query, object? parameters = null)
        {
            await Task.Delay(10);
            return new List<T> { Activator.CreateInstance<T>() };
        }

        public async Task<int> ExecuteAsync(string query, object? parameters = null)
        {
            await Task.Delay(10);
            return 1;
        }
    }
}
