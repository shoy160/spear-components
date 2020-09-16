﻿using Acb.Core.Extensions;
using Acb.Core.Helper.Http;
using Newtonsoft.Json;
using Spear.Sharp.Contracts.Dtos.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Spear.Sharp.Business.Scheduler
{
    public class JobHttp : JobBase<HttpDetailDto>
    {
        private static HttpMethod GetHttpMethod(int method)
        {
            switch (method)
            {
                case 0:
                    return HttpMethod.Get;
                case 1:
                    return HttpMethod.Post;
                case 2:
                    return HttpMethod.Delete;
                case 3:
                    return HttpMethod.Put;
                case 4:
                    return HttpMethod.Options;
                default:
                    return HttpMethod.Get;
            }
        }

        /// <summary> 执行任务 </summary>
        /// <param name="data"></param>
        /// <param name="record"></param>
        /// <returns></returns>
        protected override async Task ExecuteJob(HttpDetailDto data, JobRecordDto record)
        {
            var req = new HttpRequest(data.Url)
            {
                BodyType = (HttpBodyType)data.BodyType,

                Headers = new Dictionary<string, string>
                {
                    {"Request-By", "spear"}
                }
            };
            if (!string.IsNullOrWhiteSpace(data.Data))
                req.Data = JsonConvert.DeserializeObject(data.Data);
            if (data.Header != null && data.Header.Any())
            {
                foreach (var header in data.Header)
                {
                    try
                    {
                        if (req.Headers.ContainsKey(header.Key))
                            continue;
                        req.Headers.Add(header.Key, header.Value);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex.Message);
                    }
                }
            }
            var resp = await HttpHelper.Instance.RequestAsync(GetHttpMethod(data.Method), req);
            var html = await resp.ReadAsStringAsync();
            record.ResultCode = (int)resp.StatusCode;
            record.Result = html;
        }
    }
}
