using System.Data;

namespace BasePlatform.Infrastructure.Persistence.Dapper;

public interface IDapperQueryConnection
{
    IDbConnection CreateConnection();
}