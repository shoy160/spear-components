using Spear.AutoMapper;
using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using Spear.Dapper;
using Spear.Dapper.Domain;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Contracts.Dtos.Account;
using System.Threading.Tasks;

namespace Spear.Sharp.Business.Domain.Repositories
{
    public class AccountRepository : DapperRepository<TAccount>
    {
        public async Task<bool> ExistsAccountAsync(string account)
        {
            using (var conn = GetConnection())
                return await conn.ExistsWhereAsync<TAccount>("[account]=@account", new { account });
        }

        /// <summary> 查询账号 </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<TAccount> QueryAccountAsync(string account)
        {
            using (var conn = GetConnection())
                return await conn.QueryByIdAsync<TAccount>(account, nameof(TAccount.Account));
        }

        public async Task<AccountDto> LoginAsync(string account, string password)
        {
            var model = await QueryAccountAsync(account);
            if (model == null)
                throw new BusiException("登录帐号不存在");
            if (!string.Equals($"{model.Password},{model.PasswordSalt}".Md5(), password))
                throw new BusiException("登录密码不正确");
            return model.MapTo<AccountDto>();
        }

        public async Task<int> UpdateAsync(TAccount model)
        {
            return await TransConnection.UpdateAsync(model, new[] { nameof(TAccount.Nick), nameof(TAccount.Avatar) }, Trans);
        }
    }
}
