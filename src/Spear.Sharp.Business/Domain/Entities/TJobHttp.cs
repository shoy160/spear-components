using System;
using Spear.Core.Domain.Entities;
using Spear.Core.Serialize;

namespace Spear.Sharp.Business.Domain.Entities
{
    ///<summary> t_job_http </summary>
    [Naming("t_job_http", NamingType = NamingType.UrlCase)]
    public class TJobHttp : BaseEntity<string>
    {
        ///<summary> Id </summary>
        public override string Id { get; set; }

        ///<summary> Url </summary>
        public string Url { get; set; }

        ///<summary> 请求方式 </summary>
        public int Method { get; set; }

        ///<summary> 数据类型 </summary>
        public int BodyType { get; set; }

        ///<summary> 请求头 </summary>
        public string Header { get; set; }

        ///<summary> 请求数据 </summary>
        public string Data { get; set; }
    }
}
