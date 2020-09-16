using Spear.Sharp.Contracts.Enums;
using System;

namespace Spear.Sharp.Contracts.Dtos.Job
{
    public class TriggerDto : TriggerInputDto
    {
        public string Id { get; set; }
        public string JobId { get; set; }
        /// <summary> 上次执行 </summary>
        public DateTime? PrevTime { get; set; }
        /// <summary> 下次执行 </summary>
        public DateTime? NextTime { get; set; }
        /// <summary> 状态 </summary>
        public TriggerStatus Status { get; set; }
        /// <summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
    }
}
