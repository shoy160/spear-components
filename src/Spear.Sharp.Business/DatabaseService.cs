using Acb.Core;
using Acb.Core.Exceptions;
using Acb.Core.Helper;
using Acb.Core.Timing;
using Acb.Dapper;
using Spear.Sharp.Business.Database;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Business.Domain.Repositories;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;
using System;
using System.Threading.Tasks;

namespace Spear.Sharp.Business
{
    public class DatabaseService : IDatabaseContract
    {
        private readonly DataBaseRepository _repository;

        public DatabaseService(DataBaseRepository repository)
        {
            _repository = repository;
        }

        public async Task<int> AddAsync(Guid accountId, string name, string code, ProviderType provider, string connectionString)
        {
            if (await _repository.ExistsCodeAsync(code))
                throw new BusiException("编码已存在");
            var model = new TDatabase
            {
                Id = IdentityHelper.NewSequentialGuid(),
                AccountId = accountId,
                Name = name,
                Code = code,
                Provider = (byte)provider,
                ConnectionString = connectionString,
                CreateTime = Clock.Now,
                Status = (byte)CommonStatus.Normal
            };
            return await _repository.InsertAsync(model);
        }

        public async Task<DatabaseTablesDto> GetAsync(string key)
        {
            TDatabase model;
            if (Guid.TryParse(key, out var id))
                model = await _repository.QueryByIdAsync(id);
            else
                model = await _repository.QueryByIdAsync(key, nameof(TDatabase.Code));
            var provider = (ProviderType)model.Provider;
            var uw = new UnitOfWork(model.ConnectionString, provider.ToString());
            var service = uw.Service();
            var tables = await service.GetTablesAsync();
            return new DatabaseTablesDto
            {
                Name = model.Name,
                DbName = service.DbName,
                Provider = service.Provider,
                Tables = tables
            };
        }

        public async Task<PagedList<DatabaseDto>> PagedListAsync(Guid accountId, string keyword = null,
            ProviderType? type = null, int page = 1, int size = 10)
        {
            return await _repository.PagedListAsync(accountId, keyword, type, page, size);
        }

        public async Task<int> SetAsync(Guid id, string name, string code, ProviderType type, string connectionString)
        {
            return await _repository.UpdateAsync(id, name, code, type, connectionString);
        }

        public async Task<int> RemoveAsync(Guid id)
        {
            return await _repository.UpdateStatusAsync(id, CommonStatus.Delete);
        }

        public string ConvertToLanguageType(string dbType, ProviderType provider, LanguageType language, bool isNullable = false)
        {
            return DbTypeConverter.Instance.Convert(provider, language, dbType, isNullable);
        }

        public string ConvertToDbType(string languageType, ProviderType provider, LanguageType language)
        {
            var dbType = DbTypeConverter.Instance.DbType(provider, language, languageType);
            if (languageType.EndsWith("?"))
                dbType += "?";
            return dbType;
        }
    }
}
