using Spear.Core;
using Spear.Core.Extensions;
using Spear.Dapper;
using Spear.Dapper.Domain;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Spear.Sharp.Business.Domain.Repositories
{
    public class DataBaseRepository : DapperRepository<TDatabase>
    {
        public async Task<PagedList<DatabaseDto>> PagedListAsync(string accountId, string keyword = null, ProviderType? type = null, int page = 1,
            int size = 10)
        {
            SQL sql = Select("[account_id]=@accountId AND [status] <> 4");
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql += "AND ([code]=@code OR [name] like @keyword)";
                sql = sql["keyword", $"%{keyword}%"]["code", keyword];
            }
            if (type.HasValue)
            {
                sql += "AND [provider]=@type";
            }

            sql += "ORDER BY [create_time] DESC";

            return await sql.PagedListAsync<DatabaseDto>(Connection, page, size, new
            {
                accountId,
                type
            });
        }

        public async Task<int> UpdateAsync(string id, string name, string code, ProviderType type,
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

        public async Task<int> UpdateStatusAsync(string id, CommonStatus status)
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
