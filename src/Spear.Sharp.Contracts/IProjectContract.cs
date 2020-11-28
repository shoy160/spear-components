using Spear.Core;
using Spear.Core.Dependency;
using Spear.Sharp.Contracts.Dtos;
using System;
using System.Threading.Tasks;

namespace Spear.Sharp.Contracts
{
    /// <inheritdoc />
    /// <summary> 项目相关契约 </summary>
    public interface IProjectContract : IDependency
    {
        /// <summary> 添加项目 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> AddAsync(ProjectDto dto);

        /// <summary> 更新项目 </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> UpdateAsync(ProjectDto dto);

        /// <summary> 获取项目 </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<ProjectDto> DetailByCodeAsync(string code);

        Task<ProjectDto> DetailAsync(string id);

        /// <summary> 项目列表 </summary>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PagedList<ProjectDto>> PagedListAsync(int page = 1, int size = 10);
    }
}
