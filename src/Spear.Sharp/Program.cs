﻿using System.Threading.Tasks;
using Spear.WebApi;

namespace Spear.Sharp
{
    public class Program : DHost<Startup>
    {
        public static async Task Main(string[] args)
        {
            Builder += UseIIS;
            await Start(args);
        }
    }
}
