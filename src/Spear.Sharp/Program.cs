using Acb.WebApi;
using System.Threading.Tasks;

namespace Spear.Sharp
{
    public class Program : DHost<Startup>
    {
        public static async Task Main(string[] args)
        {
            await Start(args);
        }
    }
}
