using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using Uranometrical.OpenMantleProxy.Hosts;

namespace Uranometrical.OpenMantleProxy.Commands
{
    [Command("clean", Description = Description)]
    public class CleanHostsCommand : ICommand
    {
        public const string Description = "Wipes any redirects pointing away from \"s.optifine.net\"." +
                                          "\n                    This reverts any edits by any cape servers.";

        public async ValueTask ExecuteAsync(IConsole console)
        {
            await console.Output.WriteLineAsync("Cleaning hosts file...");

            new HostsFileEditor().Write(contents => contents.Where(x => !x.EndsWith(" s.optifine.net")));

            await console.Output.WriteLineAsync("Cleaned host file!");
        }
    }
}