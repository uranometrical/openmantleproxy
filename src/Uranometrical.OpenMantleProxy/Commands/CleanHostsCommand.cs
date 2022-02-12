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
                                          "This reverts any edits by any cape servers.";
        
        public async ValueTask ExecuteAsync(IConsole console)
        {
            await console.Output.WriteLineAsync("Cleaning hosts file...");
            
            using HostsFileEditor editor = new();
            editor.Write((writer, contents) =>
            {
                foreach (string line in contents.Where(x => !x.EndsWith(" s.optifine.net")))
                    writer.WriteLine(line);
            });

            await console.Output.WriteLineAsync("Cleaned host file!");
        }
    }
}