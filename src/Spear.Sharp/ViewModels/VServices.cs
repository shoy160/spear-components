using System.Collections.Generic;

namespace Spear.Sharp.ViewModels
{
    /// <summary> 服务 </summary>
    public class VServices
    {
        /// <summary> 服务名称 </summary>
        public string Name { get; set; }
        /// <summary> 服务标签 </summary>
        public string[] Tags { get; set; }

        /// <summary> 服务数量 </summary>
        public int Count => Services?.Count ?? 0;
        /// <summary> 服务列表 </summary>
        public List<VServiceDetail> Services { get; set; }
    }

    /// <summary> 服务详情 </summary>
    public class VServiceDetail
    {
        /// <summary> ID </summary>
        public string Id { get; set; }
        /// <summary> 地址 </summary>
        public string Address { get; set; }
        /// <summary> 端口 </summary>
        public int Port { get; set; }
        /// <summary> 标签 </summary>
        public string[] Tags { get; set; }
        public object Meta { get; set; }
    }
}
