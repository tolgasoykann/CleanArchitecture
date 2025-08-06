using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDataAccess
    {
        Task<IEnumerable<T>> ExecuteReaderAsync<T>(string sql, Func<IDataReader, T> map, object? parameters = null);
        Task<int> ExecuteNonQueryAsync(string sql, object? parameters = null);
    }
}
