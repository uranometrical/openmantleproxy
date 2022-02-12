using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CliFx;
using CliFx.Attributes;
using CliFx.Extensibility;
using CliFx.Infrastructure;

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

        public const string Description = "Starts a proxy server to redirect any and all s.optifine.net HTTP requests." +
                                          "\n                    Also modifies the \"hosts\" file to handle redirecting." +
                                          "\n                    Run the \"clean\" command after closing this to reset.";

        [CommandOption("mode", 'm', Description = "The proxy mode to use.", Converter = typeof(ProxyModeConverter))]
        public ProxyMode Mode { get; init; } = ProxyMode.Mantle;

        [CommandOption("servers", 's', Description = "The servers to use. Only used if \"mode\" is set to Custom.")]
        public string[] Servers { get; init; } = Array.Empty<string>();
        
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
        }
    }
}