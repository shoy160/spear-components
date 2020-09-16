using Acb.Core.Domain.Dtos;
using System;
using Spear.Sharp.Contracts.Enums;

namespace Spear.Sharp.Contracts.Dtos.Account
{
    public class AccountInputDto : DDto
    {
        ///<summary> 帐号 </summary>
        public string Account { get; set; }

        ///<summary> 昵称 </summary>
        public string Nick { get; set; }

        ///<summary> 头像 </summary>
        public string Avatar { get; set; }

        ///<summary> 角色 </summary>
        public AccountRole Role { get; set; }

        public string Password { get; set; }

        ///<summary> 项目ID </summary>
        public string ProjectId { get; set; }
    }
}
