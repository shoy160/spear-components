using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Extensions;
using Acb.Core.Serialize;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spear.Sharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Spear.Sharp.Controllers
{
    /// <summary> 服务注册 </summary>
    [Route("api/services")]
    public class ServicesController : DController
    {
        private readonly IHttpClientFactory _clientFactory;

        public ServicesController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        private Uri GetRequestUri(string api)
        {
            var url = "services:url".Config<string>();
            var token = "services:token".Config<string>();
            var uri = new Uri(new Uri(url), api);
            if (!string.IsNullOrWhiteSpace(token))
                uri = new Uri(uri, $"?token={token}");
            return uri;
        }

        private async Task<Dictionary<string, string[]>> GetServicesAsync()
        {
            var uri = GetRequestUri("/v1/catalog/services");
            var client = _clientFactory.CreateClient();
            var resp = await client.GetAsync(uri);
            if (!resp.IsSuccessStatusCode)
                throw new BusiException("Consul配置异常");
            var content = await resp.Content.ReadAsStringAsync();
            return JsonHelper.Json<Dictionary<string, string[]>>(content);
        }

        private async Task<List<VServiceDetail>> GetServiceDetailsAsync(string name)
        {
            var uri = GetRequestUri($"/v1/catalog/service/{name}");
            var client = _clientFactory.CreateClient();
            var resp = await client.GetAsync(uri);
            if (!resp.IsSuccessStatusCode)
                throw new BusiException("Consul配置异常");
            var content = await resp.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<dynamic>(content);
            var list = new List<VServiceDetail>();
            foreach (var item in data)
            {
                var d = new VServiceDetail
                {
                    Id = item.ServiceID,
                    Address = item.ServiceAddress,
                    Port = item.ServicePort,
                    Tags = (item.ServiceTags as JArray)?.ToObject<string[]>(),
                    Meta = item.ServiceMeta
                };
                list.Add(d);
            }

            return list;
        }

        /// <summary> 注销服务 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<DResult> DeregistServiceAsync(string id)
        {
            var uri = GetRequestUri($"/v1/agent/service/deregister/{id}");
            var client = _clientFactory.CreateClient();
            var resp = await client.SendAsync(new HttpRequestMessage(HttpMethod.Put, uri));
            if (resp.IsSuccessStatusCode)
                return Success;
            return Error(resp.StatusCode.ToString());
        }

        private async Task RegistServiceAsync()
        {
            await Task.CompletedTask;
        }

        /// <summary> 获取服务列表 </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<DResults<VServices>> List()
        {
            var list = await GetServicesAsync();
            var services = new List<VServices>();
            foreach (var item in list)
            {
                if (item.Key == "consul")
                    continue;
                var service = new VServices
                {
                    Name = item.Key,
                    Tags = item.Value,
                    Services = await GetServiceDetailsAsync(item.Key)
                };
                services.Add(service);
            }

            services = services.OrderBy(t => t.Name).ToList();

            return Succ(services, -1);
        }
    }
}
