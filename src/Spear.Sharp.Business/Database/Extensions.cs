using Acb.Core.Domain;
using Spear.Sharp.Business.Database.Services;
using MySql.Data.MySqlClient;
using Npgsql;
using System.Data.SQLite;

namespace Spear.Sharp.Business.Database
{
    public static class Extensions
    {
        public static IDbService Service(this IUnitOfWork unitOfWork, string dbSchema = null)
        {
            switch (unitOfWork.Connection)
            {
                case NpgsqlConnection _:
                    return new PostgreSqlService(unitOfWork, dbSchema);
                case MySqlConnection _:
                    return new MySqlService(unitOfWork);
                case SQLiteConnection _:
                    return new SqliteService(unitOfWork);
            }

            return new MsSqlService(unitOfWork);
        }
    }
}
