using Spear.Core;
using Spear.Core.Exceptions;
using Spear.Core.Extensions;
using Spear.Core.Helper;
using Spear.Core.Timing;
using Spear.Dapper;
using Spear.Sharp.Business.Database;
using Spear.Sharp.Business.Domain.Entities;
using Spear.Sharp.Business.Domain.Repositories;
using Spear.Sharp.Contracts;
using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Spear.Sharp.Business
{
    public class DatabaseService : IDatabaseContract
    {
        private readonly DataBaseRepository _repository;
        private const string PLACE_PASSWORD = "(password)=xxx;";

        public DatabaseService(DataBaseRepository repository)
        {
            _repository = repository;
        }

        /// <summary> 数据库密码脱敏 </summary>
        /// <param name="dto"></param>
        private void HidePassword(DatabaseDto dto)
        {
            if (dto == null || dto.ConnectionString.IsNullOrEmpty())
                return;
            dto.ConnectionString = dto.ConnectionString.Replace("(password)=[^;]+;", "$1=xxx;", RegexOptions.IgnoreCase);
        }

        /// <summary> 数据库密码还原 </summary>
        /// <param name="model"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private string RestorePassword(TDatabase model, string connectionString)
        {
            if (connectionString.IsMatch(PLACE_PASSWORD, RegexOptions.IgnoreCase))
            {
                var password = model.ConnectionString.Match("password=([^;]+);", 1, RegexOptions.IgnoreCase);
                return connectionString.Replace(PLACE_PASSWORD, $"$1={password};", RegexOptions.IgnoreCase);
            }
            return connectionString;
        }

        public async Task<int> AddAsync(string accountId, string name, string code, ProviderType provider, string connectionString)
        {
            if (await _repository.ExistsCodeAsync(code))
                throw new BusiException("编码已存在");
            var model = new TDatabase
            {
                Id = IdentityHelper.Guid32,
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
            if (key.IsMatch("^[0-9a-zA-Z]{32}$"))
                model = await _repository.QueryByIdAsync(key);
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

        public async Task<PagedList<DatabaseDto>> PagedListAsync(string accountId, string keyword = null,
            ProviderType? type = null, int page = 1, int size = 10)
        {
            var paged = await _repository.PagedListAsync(accountId, keyword, type, page, size);
            if (!paged.List.IsNullOrEmpty())
            {
                paged.List.Foreach(t => HidePassword(t));
            }
            return paged;
        }

        public async Task<int> SetAsync(string id, string name, string code, ProviderType type, string connectionString)
        {
            var model = await _repository.QueryByIdAsync(id);
            if (model == null)
                throw new BusiException("数据库连接不存在");
            connectionString = RestorePassword(model, connectionString);
            return await _repository.UpdateAsync(id, name, code, type, connectionString);
        }

        public async Task<int> RemoveAsync(string id)
        {
            return await _repository.UpdateStatusAsync(id, CommonStatus.Delete);
        }

        public string ConvertToLanguageType(ColumnDto column, ProviderType provider, LanguageType language)
        {
            return DbTypeConverter.Instance.Convert(provider, language, column);
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
