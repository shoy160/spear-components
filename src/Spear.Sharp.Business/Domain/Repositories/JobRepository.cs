using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Data;
using Acb.Core.Extensions;
using Acb.Dapper;
using Acb.Dapper.Domain;
using Dapper;
using Newtonsoft.Json;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Contracts.Dtos.Job;
using Spear.Sharp.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spear.Sharp.Business.Domain.Repositories
{
    /// <summary> 任务仓储 </summary>
    public class JobRepository : DapperRepository<TJob>
    {
        /// <summary> 查询所有任务 </summary>
        /// <returns></returns>
        public async Task<PagedList<TJob>> QueryPagedAsync(string projectId = null, string keyword = null,
            JobStatus? status = null, int page = 1, int size = 10)
        {
            SQL sql = Select("1=1");
            if (projectId.IsNotNullOrEmpty())
            {
                sql = (sql + "AND [project_id]=@projectId")["projectId", projectId];
            }

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                sql = (sql + "AND ([name] LIKE @keywork OR [group] LIKE @keyword)")["keyword", $"%{keyword}%"];
            }

            if (status.HasValue)
            {
                sql = (sql + "AND [status]=@status")["status", status];
            }
            else
            {
                sql = (sql + "AND [status]<>@status")["status", JobStatus.Delete];
            }

            sql += "ORDER BY [group],[create_time] DESC";
            var sqlStr = Connection.FormatSql(sql.ToString());
            using (var conn = GetConnection())
            {
                return await conn.PagedListAsync<TJob>(sqlStr, page, size, sql.Parameters());
            }
        }

        /// <summary> 查询Http任务 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        public Task<IEnumerable<TJobHttp>> QueryHttpDetailsAsync(IEnumerable<string> jobIds)
        {
            string sql = Select<TJobHttp>("[id] in @jobIds");
            var fmtSql = Connection.FormatSql(sql);
            return Connection.QueryAsync<TJobHttp>(fmtSql, new { jobIds = jobIds.ToArray() });
        }

        /// <summary> 查询触发器 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        public async Task<IDictionary<string, DateTime?>> QueryTimesAsync(IEnumerable<string> jobIds)
        {
            const string sql =
                "SELECT [job_id],MAX([start_time]) as [PrevTime] FROM [t_job_record] WHERE [job_id] in @jobIds GROUP BY [job_id]";
            var list = await Connection.QueryAsync<TJobTrigger>(Connection.FormatSql(sql),
                new { jobIds = jobIds.ToArray() });
            return list.ToDictionary(k => k.JobId, v => v.PrevTime);
        }

        /// <summary> 查询Http任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<TJobHttp> QueryHttpDetailByIdAsync(string jobId)
        {
            string sql = Select<TJobHttp>("[id] = @jobId");
            var fmtSql = Connection.FormatSql(sql);
            return await Connection.QueryFirstOrDefaultAsync<TJobHttp>(fmtSql, new { jobId });
        }

        /// <summary> 添加任务 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<int> InsertAsync(JobDto dto)
        {
            var job = dto.MapTo<TJob>();
            return Transaction(async (conn, trans) =>
            {
                var count = await conn.InsertAsync(job, trans: trans);
                switch (dto.Type)
                {
                    case JobType.Http:
                        var detail = new TJobHttp
                        {
                            Id = dto.Detail.Id,
                            Method = dto.Detail.Method,
                            Url = dto.Detail.Url,
                            BodyType = dto.Detail.BodyType,
                            Data = dto.Detail.Data
                        };
                        if (dto.Detail.Header != null && dto.Detail.Header.Any())
                            detail.Header = JsonConvert.SerializeObject(dto.Detail.Header);

                        count += await conn.InsertAsync(detail, trans: trans);
                        break;
                }
                return count;
            });
        }

        /// <summary> 查询任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task<JobDto> QueryByIdAsync(string jobId)
        {
            var dto = (await Connection.QueryByIdAsync<TJob>(jobId)).MapTo<JobDto>();
            switch (dto.Type)
            {
                case JobType.Http:
                    var model = await QueryHttpDetailByIdAsync(jobId);
                    dto.Detail = model.MapTo<HttpDetailDto>();
                    if (!string.IsNullOrWhiteSpace(model.Header))
                        dto.Detail.Header = JsonConvert.DeserializeObject<IDictionary<string, string>>(model.Header);
                    break;
            }
            return dto;
        }

        /// <summary> 更新任务 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<int> UpdateAsync(JobDto dto)
        {
            var job = dto.MapTo<TJob>();
            return Transaction(async (conn, trans) =>
            {
                var count = await conn.UpdateAsync(job,
                    new[]
                    {
                        nameof(TJob.Group), nameof(TJob.Name), nameof(TJob.Desc),
                        nameof(TJob.Type)
                    }, trans);
                switch (dto.Type)
                {
                    case JobType.Http:
                        var detail = new TJobHttp
                        {
                            Id = dto.Detail.Id,
                            Method = dto.Detail.Method,
                            Url = dto.Detail.Url,
                            BodyType = dto.Detail.BodyType,
                            Data = dto.Detail.Data
                        };
                        if (dto.Detail.Header != null && dto.Detail.Header.Any())
                            detail.Header = JsonConvert.SerializeObject(dto.Detail.Header);
                        count += await conn.UpdateAsync(detail,
                            new[]
                            {
                                nameof(TJobHttp.Url), nameof(TJobHttp.Method), nameof(TJobHttp.Data),
                                nameof(TJobHttp.Header), nameof(TJobHttp.BodyType)
                            }, trans);
                        break;
                }
                return count;
            });
        }

        /// <summary> 更新任务状态 </summary>
        /// <param name="jobId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Task<int> UpdateStatusAsync(string jobId, JobStatus status)
        {
            return Connection.UpdateAsync(new TJob
            {
                Id = jobId,
                Status = (byte)status
            }, new[] { nameof(TJob.Status) }, Trans);
        }

        /// <summary> 删除任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        public async Task DeleteByIdAsync(string jobId)
        {
            await Transaction(async (conn, trans) =>
            {
                await conn.DeleteAsync<TJob>(jobId, trans: trans);
                await conn.DeleteAsync<TJobHttp>(jobId, trans: trans);
                await conn.DeleteAsync<TJobTrigger>(jobId, "job_id", trans);
                await conn.DeleteAsync<TJobRecord>(jobId, "job_id", trans);
            });
        }
    }
}
