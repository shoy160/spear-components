using System.Linq;
using Acb.Core.Helper;
using Spear.Sharp.Contracts.Dtos.Database;
using Spear.Sharp.Contracts.Enums;
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

        public static string EntityCode(this TableDto table, ProviderType provider,
            LanguageType language = LanguageType.CSharp, bool isNaming = true)
        {
            var primary = table.PrimaryColumn;
            var sb = new StringBuilder();
            sb.AppendLine($"///<summary> {table.Desc()} </summary>");
            sb.AppendLine($"[{(isNaming ? "Naming" : "Table")}(\"{table.Name}\")]");
            sb.AppendLine(
                $"public class {table.ConvertedName}:BaseEntity<{primary?.LanguageType(provider, language) ?? "string"}>");
            sb.AppendLine("{");
            var index = 0;
            foreach (var column in table.Columns)
            {
                sb.AppendLine($"\t///<summary> {column.Desc()} </summary>");
                if (column.IsConverted)
                {
                    sb.AppendLine($"\t[{(isNaming ? "Naming" : "Column")}(\"{column.Name}\")]");
                }

                sb.AppendLine(
                    $"\tpublic {(column.IsPrimaryKey ? "override " : "")}{column.LanguageType(provider, language)} {column.ConvertedName} {{ get; set; }}");
                if (index < table.Columns.Count() - 1)
                    sb.AppendLine(string.Empty);
                index++;
            }

            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
