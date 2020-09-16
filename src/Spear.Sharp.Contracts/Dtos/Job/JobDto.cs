using Spear.Sharp.Contracts.Enums;
using System;

namespace Spear.Sharp.Contracts.Dtos.Job
{
    public class JobDto : JobInputDto
    {
        public string Id { get; set; }
        /// <summary> 状态 </summary>
        public JobStatus Status { get; set; }
        /// <summary> 创建时间 </summary>
        public DateTime CreationTime { get; set; }
        /// <summary> 上次执行 </summary>
        public DateTime? PrevTime { get; set; }
        /// <summary> 下次执行 </summary>
        public DateTime? NextTime { get; set; }
    }
}
