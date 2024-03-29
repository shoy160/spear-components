﻿using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Spear.Core;
using Spear.Core.Dependency;
using Spear.Core.Helper;
using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;

namespace Spear.Sharp.Business.Database
{
    public class DbTypeConverter
    {
        private readonly DbTypeMap _typeMap;
        private readonly ILogger _logger;
        private DbTypeConverter()
        {
            _logger = CurrentIocManager.CreateLogger<DbTypeConverter>();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "config", "DbTypeMaps.xml");
            _typeMap = XmlHelper.XmlDeserializeFromPath(path, new DbTypeMap());
        }

        public static DbTypeConverter Instance =>
            Singleton<DbTypeConverter>.Instance ?? (Singleton<DbTypeConverter>.Instance = new DbTypeConverter());

        /// <summary> 数据库类型转为语言类型 </summary>
        /// <param name="dbProvider">数据库驱动</param>
        /// <param name="lang">语言</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="isNullable">是否可为空</param>
        /// <returns></returns>
        public string Convert(ProviderType dbProvider, LanguageType lang, ColumnDto column)
        {
            var dbTypeMap = new DbTypeMapLange
            {
                Name = column.DbType,
                To = column.DbType
            };

            var databaseMap = _typeMap.Databases.FirstOrDefault(m => m.DbProvider == dbProvider && m.Language == lang);
            if (databaseMap == null)
            {
                _logger.LogWarning($"没有找到语言对应的映射关系:{dbProvider}->{lang}");
            }
            else
            {
                var map = databaseMap.DbTypes?.FirstOrDefault(m => m.Name == column.DbType);

                if (map == null)
                {
                    _logger.LogWarning($"没有找到语言对应的数据类型:{dbProvider}->{lang},{column.DbType}");
                }
                else
                {
                    dbTypeMap = map;
                }
            }

            if (lang == LanguageType.CSharp)
            {
                return dbTypeMap.To != "string" && column.IsNullable ? $"{dbTypeMap.To}?" : dbTypeMap.To;
            }
            if (lang == LanguageType.Java)
            {
                if (column.Name.StartsWith("is") && column.DbType == "tinyint")
                {
                    return column.IsNullable ? "Boolean" : "boolean";
                }
                if (column.IsNullable && !string.IsNullOrWhiteSpace(dbTypeMap.ToNull))
                {
                    return dbTypeMap.ToNull;
                }
                else
                {
                    return dbTypeMap.To;
                }
            }
            //:todo 其他语言
            return dbTypeMap.To;
        }

        /// <summary> 语言类型转为数据库类型 </summary>
        /// <param name="dbProvider"></param>
        /// <param name="lang"></param>
        /// <param name="languageType"></param>
        /// <returns></returns>
        public string DbType(ProviderType dbProvider, LanguageType lang, string languageType)
        {
            var dbTypeMap = new DbTypeMapLange
            {
                Name = languageType,
                To = languageType
            };
            var databaseMap = _typeMap.Databases.FirstOrDefault(m => m.DbProvider == dbProvider && m.Language == lang);
            if (databaseMap == null)
            {
                _logger.LogWarning($"没有找到语言对应的映射关系:{dbProvider}->{lang}");
            }
            var map = databaseMap?.DbTypes?.FirstOrDefault(m => m.To == languageType);

            if (map == null)
            {
                _logger.LogWarning($"没有找到语言类型对应的数据类型:{dbProvider}->{lang},{languageType}");
            }
            else
            {
                dbTypeMap = map;
            }
            return dbTypeMap.To;
        }
    }
}
