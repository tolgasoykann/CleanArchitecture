using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IDatabaseManager
    {
        Task<List<Dictionary<string, object>>> QueryAsync(string sql, Dictionary<string, object>? parameters = null);
        Task<int> ExecuteAsync(string sql, Dictionary<string, object>? parameters = null);
    }


}
