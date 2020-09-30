using Acb.Core.Helper;
using Acb.Core.Timing;
using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;
using System.Linq;
using System.Text;

namespace Spear.Sharp.Domain
{
    public static class Constants
    {
        public const string JobData = "job_data";

        private static string Desc(this DConverted dto)
        {
            return RegexHelper.ClearTrn(string.IsNullOrWhiteSpace(dto.Description) ? dto.Name : dto.Description);
        }

        private static string Space(int count)
        {
            var space = string.Empty;
            for (var i = 0; i < count; i++)
            {
                space = string.Concat(space, " ");
            }
            return space;
        }



        public static string JavaEntityCode(this TableDto table, ProviderType provider, NamingType type = NamingType.Naming)
        {
            var primary = table.PrimaryColumn;
            var sb = new StringBuilder();
            sb.AppendLine("/**");
            sb.AppendLine($" * {table.Desc()}");
            sb.AppendLine(" *");
            sb.AppendLine(" * @author {user}");
            sb.AppendLine($" * @date {Clock.Now:yyyy/MM/dd}");
            sb.AppendLine(" * @serial generate by spear ");
            sb.AppendLine(" */");
            sb.AppendLine("@Getter");
            sb.AppendLine("@Setter");
            sb.AppendLine($"@TableName(\"{table.Name}\")");
            var clsName = table.GetConvertedName();
            if (type == NamingType.Naming)
            {
                clsName = table.GetConvertedName(start: 1);
            }
            sb.AppendLine(
                $"public class {clsName}PO implements Serializable {{");
            sb.AppendLine("\tprivate static final long serialVersionUID = 1L;");
            var index = 0;
            foreach (var column in table.Columns)
            {
                sb.AppendLine("\t/**");
                sb.AppendLine($"\t * {column.Desc()}");
                sb.AppendLine("\t */");
                if (column.IsPrimaryKey)
                {
                    sb.AppendLine($"\t@TableId(value = \"{column.Name}\", type = IdType.AUTO)");
                }
                else
                {
                    sb.AppendLine($"\t@TableField(\"{column.Name}\")");
                }
                var field = column.GetConvertedName(Acb.Core.Serialize.NamingType.CamelCase);
                if (type == NamingType.Naming)
                {
                    field = column.GetConvertedName(Acb.Core.Serialize.NamingType.CamelCase, 1);
                }
                sb.AppendLine($"\tprivate {column.LanguageType(provider, LanguageType.Java)} {field};");

                if (index < table.Columns.Count() - 1)
                    sb.AppendLine(string.Empty);
                index++;
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string CSharpEntityCode(this TableDto table, ProviderType provider, NamingType type = NamingType.Naming)
        {
            var primary = table.PrimaryColumn;
            var sb = new StringBuilder();
            sb.AppendLine("///<summary>");
            sb.AppendLine($"/// {table.Desc()}");
            sb.AppendLine("/// Generate by spear");
            sb.AppendLine("///</summary>");
            if (type != NamingType.None)
            {
                sb.AppendLine($"[{(type == NamingType.Naming ? "Naming" : "Table")}(\"{table.Name}\")]");
            }

            sb.AppendLine(
                $"public class {table.ConvertedName}:BaseEntity<{primary?.LanguageType(provider, LanguageType.CSharp) ?? "string"}>");
            sb.AppendLine("{");
            var index = 0;
            foreach (var column in table.Columns)
            {
                sb.AppendLine($"\t///<summary> {column.Desc()} </summary>");
                if (column.IsConverted && type != NamingType.None)
                {
                    sb.AppendLine($"\t[{(type == NamingType.Naming ? "Naming" : "Column")}(\"{column.Name}\")]");
                }

                sb.AppendLine(
                    $"\tpublic {(column.IsPrimaryKey ? "override " : "")}{column.LanguageType(provider, LanguageType.CSharp)} {column.GetConvertedName(start: 1)} {{ get; set; }}");
                if (index < table.Columns.Count() - 1)
                    sb.AppendLine(string.Empty);
                index++;
            }

            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string EntityCode(this TableDto table, ProviderType provider,
        LanguageType language = LanguageType.CSharp, NamingType type = NamingType.Naming)
        {
            switch (language)
            {
                case LanguageType.CSharp:
                    return table.CSharpEntityCode(provider, type);
                case LanguageType.Java:
                    return table.JavaEntityCode(provider, type);
            }
            return table.CSharpEntityCode(provider, type);
        }
    }
}
