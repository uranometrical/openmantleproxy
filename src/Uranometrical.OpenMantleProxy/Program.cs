using System.Threading.Tasks;
using CliFx;

namespace Uranometrical.OpenMantleProxy
{
    internal static class Program
    {
        public static async Task<int> Main(string[] args) =>
            await new CliApplicationBuilder().AddCommandsFromThisAssembly().Build().RunAsync(args);
    }
}