using System;
using Spear.Core.Domain.Entities;
using Spear.Core.Serialize;

namespace Spear.Sharp.Business.Domain.Entities
{
    ///<summary> t_config </summary>
    [Naming("t_config", NamingType = NamingType.UrlCase)]
    public class TConfig : BaseEntity<string>
    {
        ///<summary> Id </summary>
        public override string Id { get; set; }

        ///<summary> 名称 </summary>
        public string Name { get; set; }

        ///<summary> 模式:Dev,Test,Ready,Prod... </summary>
        public string Mode { get; set; }

        ///<summary> 内容 </summary>
        public string Content { get; set; }

        ///<summary> 项目编码 </summary>
        public string ProjectId { get; set; }

        ///<summary> 配置校验码 </summary>
        public string Md5 { get; set; }

        ///<summary> 描述 </summary>
        public string Desc { get; set; }

        ///<summary> 状态:0,正常;1,历史版本;2,已删除 </summary>
        public byte Status { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime Timestamp { get; set; }
    }
}
