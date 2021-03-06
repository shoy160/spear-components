﻿using Microsoft.AspNetCore.Http.Connections.Features;
using Microsoft.AspNetCore.SignalR;
using Spear.Core.Extensions;
using Spear.Sharp.Contracts.Dtos;
using Spear.Sharp.Domain;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Spear.Core.Dependency;

namespace Spear.Sharp.Hubs
{
    public abstract class SpearHub : Hub
    {
        private const string ProjectKey = "__hub_project";
        protected readonly ILogger Logger;

        protected SpearHub()
        {
            Logger = CurrentIocManager.CreateLogger<ConfigHub>();
        }

        /// <summary> 项目编码 </summary>
        protected string Code
        {
            get
            {
                if (Context.Items.TryGetValue(ProjectKey, out var project) && project != null)
                    return ((ProjectDto)project).Code;
                return null;
            }
        }

        /// <summary> 项目ID </summary>
        protected string ProjectId
        {
            get
            {
                if (Context.Items.TryGetValue(ProjectKey, out var project) && project != null)
                    return ((ProjectDto)project).Id;
                return null;
            }
        }

        /// <summary> 获取客户端地址 </summary>
        /// <returns></returns>
        protected string RemoteAddress()
        {
            var conn = Context.Features.Get<IHttpContextFeature>()?.HttpContext?.Connection;
            return conn == null ? string.Empty : $"{conn.RemoteIpAddress}:{conn.RemotePort}";
        }

        protected virtual Task Connected() { return Task.CompletedTask; }

        /// <summary> 建立连接 </summary>
        /// <returns></returns>
        public override async Task OnConnectedAsync()
        {
            Logger.LogInformation($"hub:{Context.ConnectionId} Connected,{RemoteAddress()}");
            var context = CurrentIocManager.Resolve<IHttpContextFeature>()?.HttpContext;
            var code = context.GetProject();
            if (code == null)
                return;
            Context.Items.Add(ProjectKey, code);
            await Connected();
            await base.OnConnectedAsync();
        }

        /// <summary> 断开连接 </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Logger.LogWarning($"hub:{Context.ConnectionId} Disconnected,{RemoteAddress()},{exception.Format()}");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
