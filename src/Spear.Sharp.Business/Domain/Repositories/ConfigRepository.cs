using Spear.Core;
using Spear.Core.Data;
using Spear.Core.Domain;
using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.Timing;
using Spear.Dapper;
using Spear.Dapper.Domain;
using Dapper;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Spear.Core.Dependency;

namespace Spear.Sharp.Business.Domain.Repositories
{
    /// <summary> 配置中心仓储类 </summary>
    public class ConfigRepository : DapperRepository<TConfig>
    {
        public ConfigRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        /// <summary> 查询项目所有配置名 </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> QueryNamesAsync(string projectId)
        {
            const string sql = "SELECT [name] FROM [t_config] WHERE [project_id]=@projectId AND [status]=0 GROUP BY [name]";

            return await Connection.QueryAsync<string>(Connection.FormatSql(sql), new { projectId });
        }

        /// <summary> 查询配置 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task<string> QueryByModuleAsync(string projectId, string module, string env = null)
        {
            const string sql =
                "SELECT [content] FROM [t_config] WHERE [status]=0 AND [project_id]=@projectId AND [name]=@name AND ([mode]=@mode OR [mode] IS NULL) ORDER BY [mode]";

            using (var conn = GetConnection())
                return await conn.QueryFirstOrDefaultAsync<string>(Connection.FormatSql(sql), new
                {
                    projectId,
                    name = module,
                    mode = env
                });
        }

        /// <summary> 查询配置版本 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task<string> QueryVersionAsync(string projectId, string module, string env = null)
        {
            const string sql =
                "SELECT [md5] FROM [t_config] WHERE [status]=0 AND [project_id]=@projectId AND [name]=@name AND [status]=0 AND ([mode]=@mode OR [mode] IS NULL) ORDER BY [mode]";
            using (var conn = GetConnection())
            {
                return await conn.QueryFirstOrDefaultAsync<string>(conn.FormatSql(sql), new
                {
                    projectId,
                    name = module,
                    mode = env
                });
            }
        }

        /// <summary> 查询配置历史版本 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public async Task<PagedList<TConfig>> QueryHistoryAsync(string projectId, string module, string env = null, int page = 1, int size = 10)
        {
            string sql =
                Select("[status]=1 AND [project_id]=@projectId AND [name]=@name AND [mode]=@mode ORDER BY [timestamp] DESC");

            return await Connection.PagedListAsync<TConfig>(sql, page, size, new { projectId, name = module, mode = env });
        }

        /// <summary> 还原历史版本 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TConfig> RecoveryAsync(string id)
        {
            //更新之前版本为历史版本            
            const string updateSql = "UPDATE [t_config] SET [status]=1 WHERE [project_id]=@projectId AND [name]=@name AND [mode]=@mode AND [status]=0";
            return await Transaction(async () =>
            {
                var history = await Connection.QueryByIdAsync<TConfig>(id);
                if (history == null || history.Status != (byte)ConfigStatus.History)
                    throw new BusiException("历史版本不存在");

                var count = await TransConnection.ExecuteAsync(Connection.FormatSql(updateSql), new
                {
                    projectId = history.ProjectId,
                    name = history.Name,
                    mode = history.Mode
                }, Trans);
                //更新状态
                history.Status = (byte)ConfigStatus.Normal;
                count += await TransConnection.UpdateAsync(history, new[] { nameof(TConfig.Status) }, Trans);
                return count > 0 ? history : null;
            });
        }

        /// <summary> 保存配置 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(TConfig model)
        {
            model.Id = IdentityHelper.Guid32;
            model.Timestamp = Clock.Now;
            model.Md5 = model.Content.Md5();
            var version = await QueryVersionAsync(model.ProjectId, model.Name, model.Mode);
            if (version != null && version == model.Md5)
                throw new BusiException("配置未更改");
            //更新之前版本为历史版本
            return await UnitOfWork.Trans(async () =>
            {
                var count = await DeleteByModuleAsync(model.ProjectId, model.Name, model.Mode);
                count += await TransConnection.InsertAsync(model, trans: Trans);
                return count;
            });
        }

        /// <summary> 查询已配置的环境 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> QueryModesAsync(string projectId, string module)
        {
            const string sql =
                "SELECT [mode] FROM [t_config] WHERE [status]=@status AND [project_id]=@projectId AND [name]=@module AND [mode] IS NOT NULL";
            var fsql = Connection.FormatSql(sql);
            return await Connection.QueryAsync<string>(fsql, new { projectId, module, status = ConfigStatus.Normal });
        }

        /// <summary> 删除配置 </summary>
        /// <param name="projectId"></param>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task<int> DeleteByModuleAsync(string projectId, string module, string env)
        {
            //更新之前版本为历史版本
            SQL updateSql =
                "UPDATE [t_config] SET [status]=1 WHERE [project_id]=@projectId AND [name]=@name AND [status]=0";
            if (string.IsNullOrWhiteSpace(env))
            {
                updateSql += "AND [mode] IS NULL";
            }
            else
            {
                updateSql += "AND [mode]=@mode";
            }

            var sql = Connection.FormatSql(updateSql.ToString());
            return await TransConnection.ExecuteAsync(sql, new { projectId, name = module, mode = env },
                Trans);
        }
    }
}
