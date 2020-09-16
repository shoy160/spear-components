using Acb.Core;
using Acb.Core.Dependency;
using Spear.Sharp.Contracts.Dtos.Job;
using Spear.Sharp.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spear.Sharp.Contracts
{
    /// <inheritdoc />
    /// <summary> 任务相关契约 </summary>
    public interface IJobContract : IDependency
    {
        /// <summary> 创建任务 </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<JobDto> CreateAsync(JobInputDto inputDto);

        /// <summary> 更新任务 </summary>
        /// <param name="id"></param>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(string id, JobInputDto inputDto);

        /// <summary> 分页列表 </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<PagedList<JobDto>> PagedListAsync(JobPagedInputDto inputDto);

        /// <summary> 更新任务状态 </summary>
        /// <param name="jobId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<int> UpdateStatusAsync(string jobId, JobStatus status);

        /// <summary> 获取任务信息 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<JobDto> GetAsync(string jobId);

        /// <summary> 删除任务 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<int> RemoveAsync(string jobId);

        /// <summary> 获取Http任务详情 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<HttpDetailDto> GetHttpDetailAsync(string jobId);

        /// <summary> 批量获取Http任务详情 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        Task<IDictionary<string, HttpDetailDto>> GetHttpDetailsAsync(List<string> jobIds);

        /// <summary> 获取任务触发器 </summary>
        /// <param name="jobId"></param>
        /// <returns></returns>
        Task<List<TriggerDto>> GetTriggersAsync(string jobId);

        /// <summary> 获取任务触发器 </summary>
        /// <param name="triggerId"></param>
        /// <returns></returns>
        Task<TriggerDto> GetTriggerAsync(string triggerId);
        /// <summary> 创建触发器 </summary>
        /// <param name="jobId"></param>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<int> CreateTriggerAsync(string jobId, TriggerInputDto inputDto);
        /// <summary> 更新触发器 </summary>
        /// <param name="triggerId"></param>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<int> UpdateTriggerAsync(string triggerId, TriggerInputDto inputDto);

        /// <summary> 更新触发器状态 </summary>
        /// <param name="triggerId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task<int> UpdateTriggerStatusAsync(string triggerId, TriggerStatus status);

        /// <summary> 批量获取任务触发器 </summary>
        /// <param name="jobIds"></param>
        /// <returns></returns>
        Task<IDictionary<string, List<TriggerDto>>> GetTriggersAsync(List<string> jobIds);

        /// <summary> 添加任务记录 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> AddRecordAsync(JobRecordDto dto);

        /// <summary> 任务记录列表 </summary>
        /// <param name="jobId"></param>
        /// <param name="triggerId"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PagedList<JobRecordDto>> RecordsAsync(string jobId, string triggerId = null, int page = 1, int size = 10);
    }
}
