﻿using Spear.Core;
using Spear.Core.Dependency;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos;
using Spear.Sharp.Contracts.Enums;
using Spear.Sharp.Filters;
using Spear.Sharp.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Spear.Sharp.Controllers
{
    /// <summary> 配置中心 </summary>

    [Route("api/config")]
    public class ConfigController : DController
    {
        private readonly IConfigContract _contract;
        private readonly IHubContext<ConfigHub> _configHub;
        private readonly ILogger _logger;

        public ConfigController(IConfigContract contract, IHubContext<ConfigHub> configHub)
        {
            _contract = contract;
            _configHub = configHub;
            _logger = CurrentIocManager.CreateLogger<ConfigController>();
        }

        /// <summary> 通知配置更新 </summary>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        private void NotifyConfig(string module, string env, object config)
        {
            Task.Factory.StartNew<Task>(async () =>
            {
                using var scope = CurrentIocManager.BeginLifetimeScope();
                var hub = scope.Resolve<IHubContext<ConfigHub>>();
                if (string.IsNullOrWhiteSpace(env))
                {
                    var contract = scope.Resolve<IConfigContract>();
                    //default
                    var existsEnvs = (await contract.GetEnvsAsync(Project.Id, module)).ToList();
                    foreach (ConfigEnv configEnv in Enum.GetValues(typeof(ConfigEnv)))
                    {
                        var item = configEnv.ToString().ToLower();
                        if (existsEnvs.Contains(item))
                        {
                            continue;
                        }

                        await hub.UpdateAsync(Project.Code, module, item, config);
                    }
                }
                else
                {
                    await hub.UpdateAsync(Project.Code, module, env, config);
                }
            });
        }

        /// <summary> 获取配置 </summary>
        /// <param name="modules">多个以,分割</param>
        /// <param name="env">模式</param>
        /// <returns></returns>
        [HttpGet("/config/{modules}/{env}"), AllowGet]
        public async Task<IDictionary<string, object>> Index(string modules, string env)
        {
            var list = modules.Split(',');
            return await _contract.BatchGetAsync(Project.Id, list, env);
        }

        /// <summary> 删除配置 </summary>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        [HttpDelete("{module}/{env=default}")]
        public async Task<DResult> RemoveConfig(string module, string env)
        {
            if (env == "default") env = null;
            var result = await _contract.RemoveAsync(Project.Id, module, env);
            //:todo 通知删除 1.删除默认的，通知缺省的，2.删除具体的，获取默认再通知
            NotifyConfig(module, env, null);
            return result > 0 ? DResult.Success : DResult.Error("删除失败");
        }

        /// <summary> 获取配置 </summary>
        /// <param name="modules">多个以,分割</param>
        /// <param name="env">模式</param>
        /// <returns></returns>
        [HttpGet("version/{modules}/{env}"), AllowGet]
        public async Task<Dictionary<string, string>> Versions(string modules, string env)
        {
            var list = modules.Split(',');
            var dict = new Dictionary<string, string>();
            foreach (var model in list)
            {
                var version = await _contract.GetVersionAsync(Project.Id, model, env);
                dict.Add(model, version);
            }
            return dict;
        }

        /// <summary> 获取项目所有配置 </summary>
        [HttpGet("list")]
        public async Task<DResults<string>> Configs()
        {
            var names = await _contract.GetNamesAsync(Project.Id);
            return DResult.Succ(names.OrderBy(t => t), -1);
        }

        /// <summary> 获取配置历史版本 </summary>
        /// <param name="module"></param>
        /// <param name="env"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet("history/{module}/{env?}")]
        public async Task<DResults<ConfigDto>> History(string module, string env = null, int page = 1, int size = 10)
        {
            var histories = await _contract.GetHistoryAsync(Project.Id, module, env, page, size);
            return Succ(histories.List, histories.Total);
        }

        /// <summary> 还原历史版本 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("history/{id}")]
        public async Task<DResult> RecoveryHistory(string id)
        {
            var result = await _contract.RecoveryAsync(id);
            if (result == null)
                return DResult.Error("还原版本失败");
            NotifyConfig(result.Name, result.Mode, result.Config);
            return DResult.Success;
        }

        /// <summary> 删除历史版本 </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("history/{id}")]
        public async Task<DResult> RemoveHistory(string id)
        {
            var dto = await _contract.DetailAsync(id);
            if (dto == null)
                return DResult.Error("版本不存在");
            if (dto.Status != ConfigStatus.History)
                return DResult.Error("只能删除历史版本");
            var result = await _contract.RemoveAsync(id);
            return result > 0 ? DResult.Success : DResult.Error("删除失败");
        }

        /// <summary> 保存配置 </summary>
        /// <param name="module"></param>
        /// <param name="config"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        [HttpPost("{module}/{env?}")]
        public async Task<DResult> Save(string module, [FromBody]object config, string env = null)
        {
            var envs = Enum.GetValues(typeof(ConfigEnv)).Cast<ConfigEnv>().Select(t => t.ToString().ToLower()).ToList();
            if (!string.IsNullOrWhiteSpace(env))
            {
                env = env.ToLower();
                if (!envs.Contains(env))
                    return DResult.Error($"不支持的配置模式:{env}");
            }
            var model = new ConfigDto
            {
                ProjectId = Project.Id,
                Name = module,
                Mode = env,
                Status = (byte)ConfigStatus.Normal,
                Content = JsonConvert.SerializeObject(config)
            };
            var result = await _contract.SaveAsync(model);
            if (result <= 0)
                return DResult.Error("保存配置失败");
            NotifyConfig(module, env, config);
            return DResult.Success;
        }


    }
}