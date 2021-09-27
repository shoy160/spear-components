using Spear.Core;
using Spear.Core.Dependency;
using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Spear.Sharp.Contracts
{
    /// <inheritdoc />
    /// <summary> 数据库相关契约 </summary>
    public interface IDatabaseContract : IDependency
    {
        /// <summary> 添加数据库连接 </summary>
        /// <param name="accountId"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="provider"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        Task<int> AddAsync(string accountId, string name, string code, ProviderType provider, string connectionString);

        /// <summary> 获取数据表 </summary>
        /// <param name="key">id或者code</param>
        /// <returns></returns>
        Task<DatabaseTablesDto> GetAsync(string key);
        /// <summary> 数据库列表 </summary>
        /// <param name="accountId"></param>
        /// <param name="keyword"></param>
        /// <param name="type"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PagedList<DatabaseDto>> PagedListAsync(string accountId, string keyword = null, ProviderType? type = null,
            int page = 1, int size = 10);

        /// <summary> 更新数据库配置 </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="code"></param>
        /// <param name="type"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        Task<int> SetAsync(string id, string name, string code, ProviderType type, string connectionString);

        /// <summary> 删除数据库配置 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<int> RemoveAsync(string id);

        /// <summary> 转换数据类型 </summary>
        /// <param name="dbType"></param>
        /// <param name="provider"></param>
        /// <param name="language"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        string ConvertToLanguageType(ColumnDto column, ProviderType provider, LanguageType language);

        /// <summary> 转换数据类型 </summary>
        /// <param name="languageType"></param>
        /// <param name="provider"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        string ConvertToDbType(string languageType, ProviderType provider, LanguageType language);
    }
}
