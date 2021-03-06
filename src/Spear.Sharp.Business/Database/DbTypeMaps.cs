﻿using System.Collections.Generic;
using System.Xml.Serialization;
using Spear.Sharp.Contracts.Enums;

namespace Spear.Sharp.Business.Database
{
    public class DbTypeMap
    {
        [XmlElement("Database")]
        public List<DatabaseMap> Databases { get; set; }
    }

    public class DatabaseMap
    {
        [XmlAttribute]
        public ProviderType DbProvider { get; set; }
        [XmlAttribute]
        public LanguageType Language { get; set; }
        [XmlElement("DbType")]
        public List<DbTypeMapLange> DbTypes { get; set; }
    }
    public class DbTypeMapLange
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string To { get; set; }

        /// <summary>
        /// 空类型
        /// </summary>
        [XmlAttribute]
        public string ToNull { get; set; }
    }
}
