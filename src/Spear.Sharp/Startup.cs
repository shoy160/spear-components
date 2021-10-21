using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Spear.Core.Data;
using Spear.Core.Extensions;
using Spear.Dapper.Adapters;
using Spear.Dapper.Mysql;
using Spear.Dapper.PostgreSql;
using Spear.Dapper.SQLite;
using Spear.Sharp.Contracts;
using Spear.Sharp.Hubs;
using Spear.WebApi;
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

        public override void ConfigureServices(IServiceCollection services)
        {
            DbConnectionManager.AddAdapter(new SqlServerAdapter());
            DbConnectionManager.AddAdapter(new MySqlConnectionAdapter());
            DbConnectionManager.AddAdapter(new PostgreSqlAdapter());
            DbConnectionManager.AddAdapter(new SqliteConnectionAdapter());
            services.AddHttpClient();
            //services.AddRazorPages();
            services
                .AddSignalR();
            services.AddMvc();
            //.AddRedis(option =>
            //{
            //    option.ConnectionFactory = async writer =>
            //    {
            //        var config = "redis:default".Config<string>();
            //        return await ConnectionMultiplexer.ConnectAsync(config, writer);
            //    };
            //});
            base.ConfigureServices(services);
        }

        protected override void ConfigRoute(IEndpointRouteBuilder builder)
        {
            builder.MapHub<ConfigHub>("/config_hub");
            builder.MapHub<JobHub>("/job_hub");

            base.ConfigRoute(builder);
        }

        /// <summary> 配置 </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
                EnableDefaultFiles = true,
                DefaultFilesOptions = { DefaultFileNames = new[] { "index.html" } }
            });
            app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().SetIsOriginAllowed(t => true));

            var provider = app.ApplicationServices;

            provider.GetService<IHostApplicationLifetime>().ApplicationStopping.Register(async () =>
            {
                await provider.GetService<ISchedulerContract>().Stop();
            });

            base.Configure(app, env);
        }
    }
}
