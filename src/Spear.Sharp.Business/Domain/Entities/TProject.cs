using Spear.Core.Domain.Entities;
using Spear.Core.Serialize;
using System;

namespace Spear.Sharp.Business.Domain.Entities
{
    ///<summary> t_project </summary>
    [Naming("t_project", NamingType = NamingType.UrlCase)]
    public class TProject : BaseEntity<string>
    {
        ///<summary> Id </summary>
        public override string Id { get; set; }

        ///<summary> 项目名称 </summary>
        public string Name { get; set; }

        ///<summary> 安全性:0,匿名;1,管理验证;2.获取验证; </summary>
        public byte Security { get; set; }

        ///<summary> 描述 </summary>
        public string Desc { get; set; }

        ///<summary> 项目编码 </summary>
        public string Code { get; set; }

        ///<summary> 项目密钥 </summary>
        public string Secret { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }

        ///<summary> 状态 </summary>
        public byte Status { get; set; }
    }
}
