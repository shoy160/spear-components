using System;
using System.Threading.Tasks;
using Acb.Core;
using Acb.Core.Extensions;
using Acb.Dapper;
using Acb.Dapper.Domain;
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
