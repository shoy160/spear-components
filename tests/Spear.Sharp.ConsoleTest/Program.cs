using Spear.Core.Data.Config;
using Spear.Core.Extensions;
using Spear.Framework;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Spear.Sharp.Client;
using System;

namespace Spear.Sharp.ConsoleTest
{
    public class Program : ConsoleHost
    {
        private static void Main(string[] args)
        {
            MapServiceCollection += services =>
            {
                services.AddLogging(builder => { builder.AddConsole(); });
                services.AddSpear(new SpearOption
                {
                    //Host = "localhost",
                    //Port = 53454,
                    Host = "spear.local",
                    Port = 80,
                    Code = "ichebao",
                    Secret = "123456"
                });
            };

            UseServiceProvider += provider =>
            {
                provider.UseSpear(new[] { "basic", "dapper" });
            };
            Command += (cmd, container) =>
            {
                if (cmd.StartsWith("dapper:"))
                {
                    var cnf = cmd.Config<ConnectionConfig>();
                    Console.WriteLine(JsonConvert.SerializeObject(cnf));
                }
                else
                {
                    Console.WriteLine(cmd.Config<string>());
                }
            };

            Start(args);

            var client = Resolve<SpearClient>();
            client.ConfigChange += config =>
            {
                Console.WriteLine(JsonConvert.SerializeObject(config, Formatting.Indented));
            };
        }
    }
}
