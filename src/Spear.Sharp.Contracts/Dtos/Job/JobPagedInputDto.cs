using Acb.Core.Domain.Dtos;
using Spear.Sharp.Contracts.Enums;
using System;

namespace Spear.Sharp.Contracts.Dtos.Job
{
    public class JobPagedInputDto : PageInputDto
    {
        public Guid? ProjectId { get; set; }
        public string Keyword { get; set; }
        public JobStatus? Status { get; set; }
    }
}
