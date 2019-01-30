using System.ComponentModel;

namespace Spear.Sharp.Client
{
    public enum SpearType
    {
        /// <summary> 配置中心 </summary>
        [Description("config_hub")]
        Config,
        /// <summary> 任务调度 </summary>
        [Description("job_hub")]
        Jobs
    }
}
