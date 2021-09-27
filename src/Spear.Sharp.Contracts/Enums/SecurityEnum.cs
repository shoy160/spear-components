using System;

namespace Spear.Sharp.Contracts.Enums
{
    [Flags]
    public enum SecurityEnum : byte
    {
        /// <summary> 匿名 </summary>
        None = 0,
        /// <summary> 管理需要验证 </summary>
        Manage = 1,
        /// <summary> 获取需要验证 </summary>
        Get = 2
    }
}
