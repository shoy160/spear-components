using Spear.Core.Config;
using Spear.Core.Extensions;
using Spear.Core.Helper.Http;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spear.Core.Dependency;

namespace Spear.Sharp.Client
{
    public class SpearClient : IDisposable
    {
        private const string AuthorizeKey = "Authorization";
        private const string ProjectKey = "project";

        private HubConnection _configHub;
        private HubConnection _jobHub;
        private Timer _retryTimer;
        private readonly SpearOption _option;
        private readonly ILogger _logger;
        private readonly IDictionary<string, string> _headers;
        public event Action<object> ConfigChange;

        public SpearClient(SpearOption option)
        {
            _option = option;
            _logger = CurrentIocManager.CreateLogger<SpearClient>();
            _headers = new Dictionary<string, string>();
        }

        private async Task LoadTicket()
        {
            if (_option.Secret.IsNullOrEmpty())
            {
                if (_headers.ContainsKey(ProjectKey)) return;
                _headers.Add(ProjectKey, _option.Code);
            }
            else
            {
                if (_headers.ContainsKey(AuthorizeKey))
                    return;
                _logger.LogInformation("正在加载配置中心令牌");
                try
                {
                    var loginUrl = new Uri(new Uri(_option.Url), "api/account/login").AbsoluteUri;
                    var loginResp = await HttpHelper.Instance.PostAsync(loginUrl,
                        new { account = _option.Code, password = _option.Secret });
                    var data = await loginResp.Content.ReadAsStringAsync();
                    if (loginResp.IsSuccessStatusCode)
                    {
                        var json = new
                        {
                            Status = false,
                            Data = string.Empty
                        };
                        json = JsonConvert.DeserializeAnonymousType(data, json);
                        if (json.Status)
                            _headers[AuthorizeKey] = $"acb {json.Data}";
                    }
                    else
                    {
                        _logger.LogInformation(data);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{_option.Url}:{ex.Message}");
                }
            }
        }

        private HubConnection Connect(SpearType type)
        {
            var url = $"{_option.Url}/{type.GetText()}";
            return new HubConnectionBuilder()
                .WithUrl(url, opts =>
                {
                    LoadTicket().SyncRun();
                    foreach (var header in _headers)
                    {
                        opts.Headers.Add(header.Key, header.Value);
                    }
                })
                .AddNewtonsoftJsonProtocol()
                .Build();
        }

        private Task ConnectConfig(ConfigOption option)
        {
            async Task Start()
            {
                try
                {
                    await _configHub.StartAsync();
                    if (option.ConfigModules != null && option.ConfigModules.Length > 0)
                    {
                        //订阅配置更新
                        var model = string.IsNullOrWhiteSpace(option.Mode) ? "dev" : option.Mode.ToLower();
                        await _configHub.SendAsync("Subscript", option.ConfigModules, model);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"{_option.Url}:{ex.Message}");
                }
            };

            _retryTimer = new Timer(async obj =>
            {
                if (_configHub.State == HubConnectionState.Connected)
                {
                    _retryTimer.Dispose();
                    return;
                }
                _logger.LogInformation("retry connect");
                await Start();
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public async Task StartConfig(ConfigOption option)
        {
            _configHub = Connect(SpearType.Config);
            var provider = new SpearConfigProvider();
            ConfigHelper.Instance.Builder.Sources.Insert(0, provider);
            //订阅配置更新
            _configHub.On<IDictionary<string, object>>("UPDATE", configs =>
              {
                  _logger.LogInformation(configs.ToJson());
                  foreach (var config in configs)
                  {
                      provider.LoadConfig(config.Key, config.Value);
                      ConfigChange?.Invoke(config);
                  }
              });
            _configHub.Closed += async ex =>
            {
                _logger.LogInformation("connect closed");
                await ConnectConfig(option);
            };
            await ConnectConfig(option);
        }

        public async Task StartJob(JobOption option)
        {
            _jobHub = Connect(SpearType.Jobs);
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            Task.Run(async () =>
            {
                if (_configHub != null)
                    await _configHub.DisposeAsync();
                if (_jobHub != null)
                    await _jobHub.DisposeAsync();
            });
        }
    }
}
