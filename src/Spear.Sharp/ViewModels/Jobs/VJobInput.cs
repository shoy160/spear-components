using Spear.Sharp.Contracts.Dtos.Job;
using Spear.Sharp.Contracts.Enums;

namespace Spear.Sharp.ViewModels.Jobs
{
    public class VJobInput
    {
        /// <summary> 任务名 </summary>
        public string Name { get; set; }
        /// <summary> 组名 </summary>
        public string Group { get; set; }
        /// <summary> 任务描述 </summary>
        public string Desc { get; set; }
        /// <summary> 类型:0,http </summary>
        public JobType Type { get; set; }
        /// <summary> 任务详情 </summary>
        public HttpDetailDto Detail { get; set; }
    }
}
