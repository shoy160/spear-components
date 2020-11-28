using Spear.AutoMapper;
using Spear.Core;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos.Account;
using Spear.Sharp.Domain;
using Spear.Sharp.ViewModels;
using Spear.Sharp.ViewModels.Account;
using Spear.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Spear.Sharp.Controllers
{
    [Route("api/account")]
    public class AccountController : DController
    {
        private readonly IAccountContract _contract;

        public AccountController(IAccountContract accountContract)
        {
            _contract = accountContract;
        }

        /// <summary> 创建账户 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<DResult> Create([FromBody] VAccountInput input)
        {
            var dto = input.MapTo<AccountInputDto>();
            var result = await _contract.CreateAsync(dto);
            return result != null ? Success : Error("创建账户失败");
        }

        /// <summary> 项目登录 </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("login"), AllowAnonymous]
        public async Task<DResult<string>> Login([FromBody]VConfigLoginInput input)
        {
            var model = await _contract.LoginAsync(input.Account, input.Password);
            var client = new SpearTicket
            {
                Id = model.Id,
                Nick = model.Nick,
                Avatar = model.Avatar,
                Role = model.Role,
                ProjectId = model.ProjectId
            };
            var ticket = client.Ticket();
            return DResult.Succ(ticket);
        }

        /// <summary> 获取帐号信息 </summary>
        /// <returns></returns>
        [HttpGet("")]
        public Task<DResult<AccountDto>> Load()
        {
            return Task.FromResult(Succ(new AccountDto
            {
                Id = Ticket.Id,
                Nick = Ticket.Nick,
                Avatar = Ticket.Avatar,
                Role = Ticket.Role
            }));
        }
    }
}
