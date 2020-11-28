using System.Text;
using Spear.Core.Domain.Dtos;
using Spear.Core.Serialize;

namespace Spear.Sharp.Contracts.Dtos.Database
{
    public abstract class DConverted : DDto, IConvertedName
    {
        public abstract string Name { get; set; }

        public virtual string ConvertedName
        {
            get
            {
                return GetConvertedName();
            }
        }

        public virtual string GetConvertedName(NamingType type = NamingType.Normal, int start = 0)
        {
            var arr = Name.Split('_');
            var sb = new StringBuilder();
            var index = start > 0 ? start : 0;
            for (var i = index; i < arr.Length; i++)
            {
                var name = arr[i];
                if (string.IsNullOrWhiteSpace(name)) continue;
                if (type == NamingType.Normal || i > index)
                {
                    sb.Append(name.Substring(0, 1).ToUpper());
                    if (name.Length > 1)
                        sb.Append(name.Substring(1));
                }
                else
                {
                    sb.Append(name);
                }
            }
            return sb.ToString();
        }

        /// <summary> 描述 </summary>
        public abstract string Description { get; set; }

        public bool IsConverted => Name != ConvertedName;
    }
}
