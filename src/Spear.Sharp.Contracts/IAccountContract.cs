using Acb.Core;
using Acb.Core.Dependency;
using Spear.Sharp.Contracts.Dtos.Account;
using System;
using System.Threading.Tasks;

namespace Spear.Sharp.Contracts
{
    /// <summary> 账户相关契约 </summary>
    public interface IAccountContract : IDependency
    {
        /// <summary> 创建账户 </summary>
        /// <param name="inputDto"></param>
        /// <returns></returns>
        Task<AccountDto> CreateAsync(AccountInputDto inputDto);

        /// <summary> 账户登录 </summary>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<AccountDto> LoginAsync(string account, string password);

        Task<int> UpdateAsync(string id, AccountInputDto inputDto);

        Task<PagedList<AccountRecordDto>> LoginRecordsAsync(string id, int page = 1, int size = 10);
    }
}
