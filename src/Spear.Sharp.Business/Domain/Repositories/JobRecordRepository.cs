using Spear.Core;
using Spear.Core.Data;
using Spear.Core.Extensions;
using Spear.Dapper;
using Spear.Dapper.Domain;
using Dapper;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Contracts.Dtos.Job;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Spear.Core.Dependency;

namespace Spear.Sharp.Business.Domain.Repositories
{
    public class JobRecordRepository : DapperRepository<TJobRecord>
    {
        /// <summary> 查询任务记录 </summary>
        /// <param name="jobId"></param>
        /// <param name="triggerId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<PagedList<JobRecordDto>> QueryPagedByJobIdAsync(string jobId, string triggerId, int page, int size)
        {
            SQL sql = Select("[job_id]=@jobId");
            if (triggerId.IsNotNullOrEmpty())
            {
                sql += "AND [trigger_id]=@triggerId";
            }

            sql += "ORDER BY [start_time] DESC";
            using (var conn = GetConnection())
            {
                return await sql.PagedListAsync<JobRecordDto>(conn, page, size, new { jobId, triggerId });
            }
        }

        /// <summary> 添加任务日志 </summary>
        /// <param name="record"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(TJobRecord record)
        {
            const string sql =
                "UPDATE [t_job_trigger] SET [prev_time]=@start WHERE [id] = @id;" +
                "UPDATE [t_job_trigger] SET [times]=[times]-1 WHERE [id] = @id AND [type]=2 AND [times]>0;";
            var fmtSql = Connection.FormatSql(sql);
            return await Transaction(async (conn, trans) =>
            {
                try
                {
                    var count = await conn.ExecuteAsync(fmtSql, new { id = record.TriggerId, start = record.StartTime },
                        trans);
                    count += await conn.InsertAsync(record, trans: trans);
                    return count;
                }
                catch (Exception ex)
                {
                    CurrentIocManager.CreateLogger<JobRecordRepository>().LogError(ex, "添加任务日志异常");
                    throw;
                }
            });
        }
    }
}
