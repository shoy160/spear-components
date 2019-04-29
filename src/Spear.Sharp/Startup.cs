﻿using Acb.Core.Extensions;
using Acb.WebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Spear.Sharp.Contracts;
using Spear.Sharp.Hubs;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Spear.Sharp
{
    /// <summary> 启动类 </summary>
    public class Startup : DStartup
    {
        /// <summary> 构造函数 </summary>
        public Startup() : base("分布式管理中心接口文档") { }

        protected override void UseServices(IServiceProvider provider)
        {
            Task.Run(async () => { await provider.GetService<ISchedulerContract>().Start(); });
            base.UseServices(provider);
        }

        protected override void MapServices(IServiceCollection services)
        {
            services.AddHttpClient();
            base.MapServices(services);
        }

        /// <summary> 注册服务 </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            return base.ConfigureServices(services);
        }

        /// <summary> 配置 </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseHttpsRedirection();
            //app.UseStaticFiles();
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = new[] { "index.html" } }
            });
            app.UseSignalR(route =>
            {
                //配置
                route.MapHub<ConfigHub>("/config_hub");
                //定时任务
                route.MapHub<JobHub>("/job_hub");
            });
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            var provider = app.ApplicationServices;
            provider.GetService<IApplicationLifetime>().ApplicationStopping.Register(async () =>
            {
                await provider.GetService<ISchedulerContract>().Stop();
            });
            base.Configure(app, env);
        }
    }
}
