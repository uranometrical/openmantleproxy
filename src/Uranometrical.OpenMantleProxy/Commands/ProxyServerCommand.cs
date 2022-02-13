using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Extensibility;
using CliFx.Infrastructure;
using Uranometrical.OpenMantleProxy.Hosts;
using Uranometrical.OpenMantleProxy.Server;

namespace Uranometrical.OpenMantleProxy.Commands
{
    [Command("server", Description = Description)]
    public class ProxyServerCommand : ICommand
    {
        public enum ProxyMode
        {
            Mantle,
            OptiMantle,
            MantleFine,
            Custom
        }

        private class ProxyModeConverter : BindingConverter<ProxyMode>
        {
            public override ProxyMode Convert(string? rawValue)
            {
                string[] names = Enum.GetNames(typeof(ProxyMode));

                return Enum.Parse<ProxyMode>(
                    names.FirstOrDefault(
                        x => x.Equals(rawValue ?? "", StringComparison.OrdinalIgnoreCase)
                    ) ?? ProxyMode.Mantle.ToString()
                );
            }
        }

        public const string Description =
            "Starts a proxy server to redirect any and all s.optifine.net HTTP requests." +
            "\n                    Also modifies the \"hosts\" file to handle redirecting." +
            "\n                    Run the \"clean\" command after closing this to reset.";

        public const string OptiFineIp = "http://107.182.233.85";

        [CommandOption("mode", 'm', Description = "The proxy mode to use.", Converter = typeof(ProxyModeConverter))]
        public ProxyMode Mode { get; init; } = ProxyMode.Mantle;

        [CommandOption("servers", 's', Description = "The servers to use. Only used if \"mode\" is set to Custom.")]
        public string[] Servers { get; private set; } = Array.Empty<string>();

        [CommandOption("nohosts", 'n', Description = "Skip any modifications to the hosts file. For running on servers.")]
        public bool NoHosts { get; init; } = false;

        public async ValueTask ExecuteAsync(IConsole console)
        {
            await console.Output.WriteLineAsync(
                $@"
      .+=:.   .:=+.      
      +*****+*****+      
     -***=+***+=***-     
    .***=       =***.    
    +***         ***+    
   -***:         :***-   OpenMantleProxy v{typeof(ProxyServerCommand).Assembly.GetName().Version}
  .***=           =***.  by the Uranometrical Team
  +**+  .       .  ***+  
 =***.:+*=.   .=*+::***- 
.***++*****=:=*****++***.
:+*****=:+*****+:=*****+:
  :++=.   :=+=:   .=++:  
            ");

            await console.Output.WriteLineAsync("Using proxy mode: " + Mode);

            if (Mode == ProxyMode.Custom)
                await console.Output.WriteLineAsync("Custom servers: " + string.Join(", ", Servers));

            if (!NoHosts)
            {
                await console.Output.WriteLineAsync("\nModifying hosts file...");

                new HostsFileEditor().Write(contents =>
                {
                    List<string> content = contents.Where(x => !x.EndsWith(" s.optifine.net")).ToList();
                    content.Add("127.0.0.1 s.optifine.net");
                    return content;
                });

                await console.Output.WriteLineAsync("Modified hosts file!");
            }

            switch (Mode)
            {
                case ProxyMode.Mantle:
                    Servers = new[] {"http://capes.mantle.gg"};
                    break;

                case ProxyMode.OptiMantle:
                    Servers = new[] {OptiFineIp, "http://capes.mantle.gg"};

                    break;

                case ProxyMode.MantleFine:
                    Servers = new[] {"http://capes.mantle.gg", OptiFineIp};

                    break;

                case ProxyMode.Custom:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(Mode));
            }


            await console.Output.WriteLineAsync("Starting server! You can kill the server by pressing <ENTER>.");

            CapeServer server = new(Servers);
            server.Listen();

            await console.Output.WriteLineAsync("Killed server!" + (NoHosts ? "" : " Reverting hosts file..."));

            if (!NoHosts) 
                new HostsFileEditor().Write(contents => contents.Where(x => !x.EndsWith(" s.optifine.net")));

            await console.Output.WriteLineAsync("Operations completed.!");
        }
    }
}