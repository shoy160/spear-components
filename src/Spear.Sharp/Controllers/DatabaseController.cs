using Spear.AutoMapper;
using Spear.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;
using Spear.Sharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DatabaseDto = Spear.Sharp.Contracts.Dtos.Database.DatabaseDto;

namespace Spear.Sharp.Controllers
{
    /// <summary> 数据库项目接口 </summary>
    [Route("api/database")]
    public class DatabaseController : DController
    {
        private readonly IDatabaseContract _contract;

        public DatabaseController(IDatabaseContract contract)
        {
            _contract = contract;
        }

        /// <summary> 数据库连接列表 </summary>
        /// <param name="keyword"></param>
        /// <param name="type"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<DResults<VDatabase>> ListAsync(string keyword = null, ProviderType? type = null, int page = 1,
            int size = 10)
        {
            var dtos = await _contract.PagedListAsync(Ticket.Id, keyword, type, page, size);
            var models = dtos.MapPagedList<VDatabase, DatabaseDto>();
            return Succ(models.List, models.Total);
        }

        /// <summary> 添加数据库连接 </summary>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<DResult> AddAsync([FromBody] VDatabaseInput input)
        {
            var result = await _contract.AddAsync(Ticket.Id, input.Name, input.Code, input.Provider, input.ConnectionString);
            return FromResult(result, "添加数据库连接失败");
        }

        /// <summary> 编辑数据库连接 </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<DResult> EditAsync(string id, [FromBody] VDatabaseInput input)
        {
            var result = await _contract.SetAsync(id, input.Name, input.Code, input.Provider, input.ConnectionString);
            return FromResult(result, "更新数据库连接失败");
        }

        /// <summary> 删除数据库连接 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<DResult> RemoveAsync(string id)
        {
            var result = await _contract.RemoveAsync(id);
            return FromResult(result, "删除失败");
        }

        /// <summary> 数据库文档 </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("/tables/{key}"), AllowAnonymous, ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Tables(string key)
        {
            try
            {
                var dto = await _contract.GetAsync(key);
                ViewBag.DbName = dto.DbName;
                ViewBag.Provider = dto.Provider;
                ViewBag.Name = dto.Name;
                return View(dto.Tables);
            }
            catch (Exception ex)
            {
                ViewBag.DbName = ex.Message;
                ViewBag.Name = "";
                return View(new List<TableDto>());
            }
        }
    }
}
