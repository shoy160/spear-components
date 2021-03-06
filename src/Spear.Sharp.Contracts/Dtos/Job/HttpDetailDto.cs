﻿using System.Collections.Generic;

namespace Spear.Sharp.Contracts.Dtos.Job
{
    public class HttpDetailDto : JobDetailDto
    {
        /// <summary> URL </summary>
        public string Url { get; set; }
        /// <summary> 请求方式 </summary>
        public int Method { get; set; }
        /// <summary> 数据类型 </summary>
        public int BodyType { get; set; }
        /// <summary> 请求头 </summary>
        public IDictionary<string, string> Header { get; set; }
        /// <summary> 数据 </summary>
        public string Data { get; set; }
    }
}
