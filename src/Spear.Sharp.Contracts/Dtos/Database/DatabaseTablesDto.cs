using Spear.Core.Domain.Dtos;
using Spear.Sharp.Contracts.Enums;
using System.Collections.Generic;

namespace Spear.Sharp.Contracts.Dtos.Database
{
    public class DatabaseTablesDto : DDto
    {
        public string Name { get; set; }
        public string DbName { get; set; }
        public ProviderType Provider { get; set; }
        public IEnumerable<TableDto> Tables { get; set; }
    }
}
