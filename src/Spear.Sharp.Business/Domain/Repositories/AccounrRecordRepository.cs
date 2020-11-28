using System;
using System.Threading.Tasks;
using Spear.Core;
using Spear.Core.Extensions;
using Spear.Dapper;
using Spear.Dapper.Domain;
using Spear.Sharp.Business.Domain.Entities;

namespace Spear.Sharp.Business.Domain.Repositories
{
    public class AccounrRecordRepository : DapperRepository<TAccountRecord>
    {
        public Task<PagedList<TAccountRecord>> QueryPagedListAsync(string accountId, int page = 1, int size = 10)
        {
            SQL sql =
                Select("[account_id]=@accountId ORDER BY [create_time] DESC");
            return sql.PagedListAsync<TAccountRecord>(Connection, page, size, new { accountId });
        }
    }
}
