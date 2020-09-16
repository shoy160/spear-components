using Acb.AutoMapper;
using Acb.Core;
using Acb.Core.Domain;
using Acb.Core.Extensions;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Business.Domain.Repositories;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Spear.Sharp.Business
{
    public class ConfigService : DService, IConfigContract
    {
        private readonly ConfigRepository _repository;

        public ConfigService(ConfigRepository repository)
        {
            _repository = repository;
        }

        public Task<IEnumerable<string>> GetNamesAsync(string projectId)
        {
            return _repository.QueryNamesAsync(projectId);
        }

        public Task<IEnumerable<string>> GetEnvsAsync(string projectId, string module)
        {
            return _repository.QueryModesAsync(projectId, module);
        }

        public Task<string> GetAsync(string projectId, string module, string env = null)
        {
            return _repository.QueryByModuleAsync(projectId, module, env);
        }

        public async Task<IDictionary<string, object>> BatchGetAsync(string projectId, string[] modules, string env = null)
        {
            var dict = new Dictionary<string, object>();
            foreach (var model in modules)
            {
                var config = await GetAsync(projectId, model, env);
                if (config == null)
                    continue;
                var obj = JsonConvert.DeserializeObject<JObject>(config);
                var tokens = obj.Children();
                foreach (var item in tokens)
                {
                    if (item is JProperty prop)
                    {
                        dict.AddOrUpdate(prop.Name, prop.Value);
                    }
                }
            }
            return dict;
        }

        public async Task<ConfigDto> DetailAsync(string configId)
        {
            var model = await _repository.QueryByIdAsync(configId);
            return model.MapTo<ConfigDto>();
        }

        public Task<string> GetVersionAsync(string projectId, string module, string env = null)
        {
            return _repository.QueryVersionAsync(projectId, module, env);
        }

        public async Task<PagedList<ConfigDto>> GetHistoryAsync(string projectId, string module, string env = null, int page = 1, int size = 10)
        {
            var models = await _repository.QueryHistoryAsync(projectId, module, env, page, size);
            return models.MapPagedList<ConfigDto, TConfig>();
        }

        public async Task<ConfigDto> RecoveryAsync(string historyId)
        {
            var model = await _repository.RecoveryAsync(historyId);
            return model.MapTo<ConfigDto>();
        }

        public async Task<int> SaveAsync(ConfigDto dto)
        {
            return await _repository.UpdateAsync(dto.MapTo<TConfig>());
        }

        public async Task<int> RemoveAsync(string projectId, string module, string env)
        {
            return await _repository.DeleteByModuleAsync(projectId, module, env);
        }

        public Task<int> RemoveAsync(string configId)
        {
            return _repository.DeleteAsync(configId);
        }
    }
}
