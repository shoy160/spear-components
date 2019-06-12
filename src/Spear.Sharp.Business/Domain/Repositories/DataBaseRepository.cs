using Acb.Core;
using Acb.Core.Extensions;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Spear.Sharp.Business.Domain.Repositories
{
    public class DataBaseRepository : DapperRepository<TDatabase>
    {
        public async Task<PagedList<DatabaseDto>> PagedListAsync(Guid accountId, string keyword = null, ProviderType? type = null, int page = 1,
            int size = 10)
        {
            var tableType = typeof(TDatabase);
            SQL sql = $"SELECT {tableType.Columns()} FROM [{tableType.PropName()}] WHERE [AccountId]=@accountId AND [Status] <> 4";
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += "AND ([Code]=@code OR [Name] like @keyword)";
                sql = sql["keyword", $"%{keyword}%"]["code", keyword];
            }
            if (type.HasValue)
            {
                sql += "AND [Provider]=@type";
            }

            sql += "ORDER BY [CreateTime] DESC";

            return await sql.PagedListAsync<DatabaseDto>(Connection, page, size, new
            {
                accountId,
                type
            });
        }

        public async Task<int> UpdateAsync(Guid id, string name, string code, ProviderType type,
            string connectionString)
        {
            return await Connection.UpdateAsync(new TDatabase
            {
                Id = id,
                Name = name,
                Code = code,
                Provider = (byte)type,
                ConnectionString = connectionString
            },
                new[]
                {
                    nameof(TDatabase.Name), nameof(TDatabase.Code), nameof(TDatabase.Provider),
                    nameof(TDatabase.ConnectionString)
                }, Trans);
        }

        public async Task<int> UpdateStatusAsync(Guid id, CommonStatus status)
        {
            return await Connection.UpdateAsync(new TDatabase
            {
                Id = id,
                Status = (byte)status
            }, new[] { nameof(TDatabase.Status) }, Trans);
        }

        public async Task<bool> ExistsCodeAsync(string code)
        {
            return await Connection.ExistsAsync<TDatabase>(nameof(TDatabase.Code), code);
        }
    }
}
