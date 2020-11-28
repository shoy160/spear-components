﻿using Spear.Core.Domain;
using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spear.Sharp.Business.Database.Services
{
    public class MySqlService : BaseService
    {
        public MySqlService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            Provider = ProviderType.MySql;
        }

        protected override Task<IEnumerable<TableDto>> QueryTableAsync()
        {
            const string sql =
                @"SELECT T.TABLE_NAME Name,( CASE WHEN T.TABLE_TYPE = 'VIEW' THEN 'View' ELSE 'Table' END ) AS Type,
T.TABLE_COMMENT AS Description FROM Information_Schema.TABLES AS T WHERE T.Table_Schema = ?dbName; ";
            return Connection.QueryAsync<TableDto>(sql, new { dbName = Connection.Database });
        }

        protected override Task<IEnumerable<ColumnDto>> QueryColumnAsync(string table, int? tableId = null)
        {
            const string sql =
                @"SELECT Col.Column_Name AS `Name`, Col.Data_Type AS DbType,
(CASE WHEN Col.Numeric_Precision Is Not Null THEN(Col.Numeric_Precision + 1) 
When Col.Character_Maximum_Length Is Not Null Then Col.Character_Maximum_Length Else Null END ) AS DataLength,
( CASE WHEN Col.Is_Nullable = 'NO' THEN 0 ELSE 1 END ) AS IsNullable,
( CASE WHEN Col.Column_Key = 'PRI' THEN 1 ELSE 0 END ) AS IsPrimaryKey,
( CASE WHEN Col.Extra = 'auto_increment' THEN 1 ELSE 0 END ) AS AutoIncrement,Col.Column_Comment AS `Description` 
FROM Information_Schema.COLUMNS AS Col Where Table_Schema =?dbName and Table_Name =?table; ";
            return Connection.QueryAsync<ColumnDto>(sql, new { table, dbName = Connection.Database });
        }
    }
}
