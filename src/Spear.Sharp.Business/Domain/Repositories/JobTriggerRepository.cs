using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spear.Core.Data;
using Spear.Dapper;
using Spear.Dapper.Domain;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Contracts.Dtos.Job;
using Spear.Sharp.Contracts.Enums;
using Dapper;

namespace Spear.Sharp.Business.Domain.Repositories
{
    public class JobTriggerRepository : DapperRepository<TJobTrigger>
    {
        /// <summary> 查询触发器 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<TriggerDto>> QueryByJobIdAsync(string jobId)
        {
            string sql = Select("[job_id] = @jobId AND [status] <> 4 ORDER BY [create_time]");
            var fmtSql = Connection.FormatSql(sql);
            return await Connection.QueryAsync<TriggerDto>(fmtSql, new { jobId });
        }

        /// <summary> 查询触发器 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, List<TriggerDto>>> QueryByJobIdsAsync(IEnumerable<string> jobIds)
        {
            string sql = Select("[job_id] in @jobIds AND [status] <> 4");
            var list = await Connection.QueryAsync<TriggerDto>(Connection.FormatSql(sql),
                new { jobIds = jobIds.ToArray() });
            return list.GroupBy(t => t.JobId).ToDictionary(k => k.Key, v => v.ToList());
        }

        public Task<int> UpdateAsync(TJobTrigger model)
        {
            return Connection.UpdateAsync(model,
                new[]
                {
                    nameof(TJobTrigger.Type), nameof(TJobTrigger.Start), nameof(TJobTrigger.Expired),
                    nameof(TJobTrigger.Corn), nameof(TJobTrigger.Times), nameof(TJobTrigger.Interval)
                }, Trans);
        }

        public Task<int> UpdateStatusAsync(string triggerId, TriggerStatus status)
        {
            return Connection.UpdateAsync(new TJobTrigger
            {
                Id = triggerId,
                Status = (byte)status
            }, new[] { nameof(TJobTrigger.Status) }, Trans);
        }
    }
}
