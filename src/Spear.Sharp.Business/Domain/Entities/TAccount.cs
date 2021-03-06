﻿using System;
using Spear.Core.Domain.Entities;
using Spear.Core.Serialize;

namespace Spear.Sharp.Business.Domain.Entities
{
    ///<summary> 帐号表 </summary>
    [Naming("t_account", NamingType = NamingType.UrlCase)]
    public class TAccount : BaseEntity<string>
    {
        ///<summary> Id </summary>
        public override string Id { get; set; }

        ///<summary> 帐号 </summary>
        public string Account { get; set; }

        ///<summary> 密码 </summary>
        public string Password { get; set; }

        ///<summary> 密码盐 </summary>
        public string PasswordSalt { get; set; }

        ///<summary> 昵称 </summary>
        public string Nick { get; set; }

        ///<summary> 头像 </summary>
        public string Avatar { get; set; }

        ///<summary> 角色 </summary>
        public byte Role { get; set; }

        ///<summary> 创建时间 </summary>
        public DateTime CreateTime { get; set; }

        ///<summary> 最后登录时间 </summary>
        public DateTime? LastLoginTime { get; set; }

        ///<summary> 项目ID </summary>
        public string ProjectId { get; set; }
    }
}
