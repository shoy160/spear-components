using Spear.Core.Domain.Dtos;
using Spear.Sharp.Contracts.Enums;
using System;

namespace Spear.Sharp.Contracts.Dtos.Job
{
    public class TriggerInputDto : DDto
    {
        /// <summary> 触发器类型 </summary>
        public TriggerType Type { get; set; }
        /// <summary> Cron表达式 </summary>
        public string Cron { get; set; }
        /// <summary> 执行次数 </summary>
        public int Times { get; set; }
        /// <summary> 时间间隔(秒) </summary>
        public int Interval { get; set; }
        /// <summary> 开始时间 </summary>
        public DateTime? Start { get; set; }
        /// <summary> 结束时间 </summary>
        public DateTime? Expired { get; set; }
    }
}
