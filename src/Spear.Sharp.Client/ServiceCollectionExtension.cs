﻿using Spear.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Spear.Sharp.Client
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddSpear(this IServiceCollection services, SpearOption option = null)
        {
            option = option ?? new SpearOption();
            services.TryAddSingleton(provider => new SpearClient(option));
            return services;
        }

        public static IServiceProvider UseSpear(this IServiceProvider provider, string[] modules = null, string jobName = null)
        {
            var client = provider.GetService<SpearClient>();
            Task.Factory.StartNew(async () =>
            {
                if (modules != null && modules.Any())
                {
                    await client.StartConfig(new ConfigOption
                    {
                        Mode = Constants.Mode.ToString().ToLower(),
                        ConfigModules = modules
                    });
                }

                if (!string.IsNullOrWhiteSpace(jobName))
                    await client.StartJob(new JobOption());
            });
            return provider;
        }
    }
}
