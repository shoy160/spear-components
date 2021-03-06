﻿using System.Threading.Tasks;
using Spear.AutoMapper;
using Spear.Core.Exceptions;
using Spear.Core.Helper;
using Spear.Dapper;
using Spear.Dapper.Domain;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Contracts.Dtos;

namespace Spear.Sharp.Business.Domain.Repositories
{
    public class ProjectRepository : DapperRepository<TProject>
    {
        /// <summary> 添加项目 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> InsertAsync(TProject model)
        {
            model.Id = IdentityHelper.Guid32;
            if (string.IsNullOrWhiteSpace(model.Code))
            {
                var count = await Connection.CountWhereAsync<TProject>() + 1;
                model.Code = $"p{count.ToString().PadLeft(3, '0')}";
            }
            if (await Connection.ExistsAsync<TProject>("Code", model.Code))
                throw new BusiException("项目编码已存在");
            //:todo create account
            //await CheckProjectAccount(model.Code, model.Account);
            return await Connection.InsertAsync(model, trans: Trans);
        }

        /// <summary> 更新项目 </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<int> UpdateAsync(TProject model)
        {
            var project = await Connection.QueryByIdAsync<TProject>(model.Id);
            if (project == null)
                throw new BusiException("项目不存在");
            model.Id = project.Id;
            //:todo create account

            return await Connection.UpdateAsync(model,
                new[]
                {
                    nameof(TProject.Name), nameof(TProject.Security), nameof(TProject.Desc)
                }, Trans);
        }

        public async Task<ProjectDto> QueryByCodeAsync(string code)
        {
            var model = await Connection.QueryByIdAsync<TProject>(code, nameof(TProject.Code));
            return model.MapTo<ProjectDto>();
        }

        //public ProjectDto QueryByCode(string code)
        //{
        //    var model = Connection.QueryById<TProject>(code, nameof(TProject.Code));
        //    return model.MapTo<ProjectDto>();
        //}
    }
}
