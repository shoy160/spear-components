using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spear.Sharp.Business.Database
{
    public interface IDbService
    {
        string DbName { get; }
        ProviderType Provider { get; }
        Task<IEnumerable<TableDto>> GetTablesAsync();
    }
}
