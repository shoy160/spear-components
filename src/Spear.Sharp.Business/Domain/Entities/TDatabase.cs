using System;
using Spear.Core.Domain.Entities;
using Spear.Core.Serialize;

namespace Spear.Sharp.Business.Domain.Entities
{
    ///<summary> 数据库连接表 </summary>
    [Naming("t_database", NamingType = NamingType.UrlCase)]
    public class TDatabase : BaseEntity<string>
    {
        ///<summary> Id </summary>
        public override string Id { get; set; }

        ///<summary> 帐号ID </summary>
        public string AccountId { get; set; }

        ///<summary> 名称 </summary>
        public string Name { get; set; }
        public string Code { get; set; }

        ///<summary> 数据提供者,mysql,postgresql,sqlserver等 </summary>
        public byte Provider { get; set; }

        ///<summary> 数据库连接字符(需要具有权限的数据库连接) </summary>
        public string ConnectionString { get; set; }

        ///<summary> 状态 </summary>
        public byte Status { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }
    }
}
