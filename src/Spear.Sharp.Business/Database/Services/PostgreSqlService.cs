﻿using Spear.Core.Domain;
using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;
using Dapper;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spear.Sharp.Business.Database.Services
{
    public class PostgreSqlService : BaseService
    {
        private readonly string _dbSchema;

        public PostgreSqlService(IUnitOfWork unitOfWork, string dbSchema = null) : base(unitOfWork)
        {
            _dbSchema = dbSchema ?? "public";
            Provider = ProviderType.PostgreSql;
        }

        protected override Task<IEnumerable<TableDto>> QueryTableAsync()
        {
            const string sql =
                "(SELECT T.tablename AS \"Name\",'Table' AS \"Type\",obj_description(C.oid) As \"Description\" " +
                "FROM pg_tables T left join pg_class C on C.relname = T.tablename and C.relkind = 'r' WHERE schemaname = @dbSchema) " +
                "UNION ALL (SELECT V.viewname AS \"Name\", 'View' AS \"Type\", obj_description(C.oid) as \"Description\" " +
                "FROM pg_views V left join pg_class C on C.relname = V.viewname and C.relkind = 'v' WHERE schemaname = @dbSchema)";
            return Connection.QueryAsync<TableDto>(sql, new { dbSchema = _dbSchema });
        }

        protected override Task<IEnumerable<ColumnDto>> QueryColumnAsync(string table, int? tableId = null)
        {
            const string sql =
                "SELECT a.attname AS \"Name\",col.udt_name AS \"DbType\"," +
                "COALESCE(col.character_maximum_length, col.numeric_precision, -1) AS \"DataLength\",col.numeric_scale AS \"Scale\"," +
                "(CASE a.attnotnull WHEN 't' THEN 0 ELSE 1 END ) AS \"IsNullable\"," +
                "(CASE a.attnum WHEN cs.conkey[1] THEN 1 ELSE 0 END ) AS \"IsPrimaryKey\"," +
                "(CASE WHEN position( 'nextval' IN col.column_default ) > 0 THEN 1 ELSE 0 END ) AS \"AutoIncrement\"," +
                "col_description(a.attrelid, a.attnum) AS \"Description\" FROM pg_attribute a LEFT JOIN pg_class c ON a.attrelid = c.oid " +
                "LEFT JOIN pg_constraint cs ON cs.conrelid = c.oid AND cs.contype = 'p' LEFT JOIN pg_namespace n ON n.oid = c.relnamespace " +
                "LEFT JOIN information_schema.COLUMNS col ON col.table_schema = n.nspname AND col.table_name = c.relname " +
                "AND col.column_name = a.attname WHERE a.attnum > 0 AND col.udt_name IS NOT NULL AND c.relname = @table " +
                "AND n.nspname = @dbSchema order by a.attnum asc";
            return Connection.QueryAsync<ColumnDto>(sql, new { table, dbSchema = _dbSchema });
        }
    }
}
