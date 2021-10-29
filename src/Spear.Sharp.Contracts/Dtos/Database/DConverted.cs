using System.Linq;
using System.Text;
using Spear.Core.Domain.Dtos;
using Spear.Core.Extensions;
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

        public virtual string GetConvertedName(NamingType type = NamingType.Normal, bool trimPrefix = true)
        {
            var array = Name.Split('_', '-');
            if (array.Length > 1 && trimPrefix && array[0].In("t", "fd"))
                array = array.Skip(1).Select(t => t.Substring(0, 1).ToUpper() + t.Substring(1)).ToArray();
            else
                array = array.Select(t => t.Substring(0, 1).ToUpper() + t.Substring(1)).ToArray();
            string name = string.Join(string.Empty, array);
            switch (type)
            {
                case NamingType.CamelCase:
                    return name.ToCamelCase();
                case NamingType.UrlCase:
                    return name.ToUrlCase();
                default:
                    return name;
            }
        }

        /// <summary> 描述 </summary>
        public abstract string Description { get; set; }

        public bool IsConverted => Name != ConvertedName;
    }
}
