using System.Data;

namespace Infrastructure.Services.Database;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
