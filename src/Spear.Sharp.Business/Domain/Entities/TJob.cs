using System;
using Spear.Core.Domain.Entities;
using Spear.Core.Serialize;

namespace Spear.Sharp.Business.Domain.Entities
{
    ///<summary> t_job </summary>
    [Naming("t_job", NamingType = NamingType.UrlCase)]
    public class TJob : BaseEntity<string>
    {
        ///<summary> Id </summary>
        public override string Id { get; set; }

        ///<summary> 任务名称 </summary>
        public string Name { get; set; }

        ///<summary> 任务分组 </summary>
        public string Group { get; set; }

        ///<summary> 任务状态 </summary>
        public byte Status { get; set; }

        ///<summary> 任务类型 </summary>
        public byte Type { get; set; }

        ///<summary> 任务描述 </summary>
        public string Desc { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }

        ///<summary> 项目ID </summary>
        public string ProjectId { get; set; }
    }
}
