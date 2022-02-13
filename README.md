# OpenMantleProxy
Open-source alternative to the traditional Mantle program, allowing you to pull capes from any number of hosted cape servers.

To run this program, please grab the latest release from [here](https://github.com/uranometrical/openmantleproxy/releases). You will need the .NET 6 SDK, which you can get from [here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0).

Please run this *before* opening any Minecraft instances. You have to run these commands (i.e. `dotnet Uranometrical.OpenMantleProxy.dll ...`) through the command line, as well. Be sure so your cmd process with admin privileges in order to allow modifications to be made to the `hosts` file.

![gif](https://user-images.githubusercontent.com/27323911/153735613-30917e1e-1683-494d-8bf7-8cd3b3e72ed7.gif)

## Functionality
OpenMantleProxy starts a local HTTP server to process any received requests. After starting the HTTP server, OpenMantleProxy modifies the computer's `hosts` file to redirect any `s.optifine.net` (OptiFine cape server) requests to the newly-opened HTTP server. Different modes will make the redirect function differently.

Due to the requirement of modifying the computer's `hosts` file, it is more than likely that the program will require administrator privileges on any Windows operating system, and root on *Nix.

To start a server, run `dotnet Uranometrical.OpenMantleProxy.dll server`. Please read the "Modes" section to understand which one is best for you.

### Modes
Use a mode by appending `--mode` (optionally, `-m`) to the command above, followed by the mode's name.

There are four different proxy modes in OpenMantleProxy, detailed below:

#### `mantle`
`mantle` is the default mode, and reroutes all `s.optifine.net` requests to `capes.mantle.gg`. This is the same behavior as regular Mantle.

#### `optimantle`
`optimantle` initially sends an HTTP request to `s.optifine.net`. If a cape is not resolved, then an HTTP request is sent to `capes.mantle.gg`.

#### `mantlefine`
`mantlefine` functions identically to `optifine`, just in reverse.

#### `custom`
`custom` ignores any predefined servers and instead expects an additional input (`--servers`/`-s`), which it will pull from for capes instead.

Additional servers are specified in this syntax: `--servers your.cape.sever another.cape.server capes.mantle.gg s.optifine.net`. In this example, `your.cape.server` will be tried first, then `another.cape.server`, etc.

## Security
OpenMantleProxy only establishes connections to servers `capes.mantle.gg`, `s.optifine.net`, and whatever additional servers you specify. As long as these domains remain safe moving forward (essentially assuming no malicious third party takes control of either domain), there are no security risks. Always be safe while installing software online, however, as this program does require elevated permissions. If you do not feel a binary release is safe, please compile this program manually after throughly examining its code.

## Safety
OpenMantleProxy can be used with any Minecraft client that uses OptiFine without fear of breaking the client's Terms of Service. Clients such as Lunar like to claim that Mantle violates their terms, but it doesn't, as Mantle does not touch Lunar. It modifies core OS functionalities. This is the same for OpenMantleProxy.

Mantle's terms do not prevent third-party programs from accessing their website or its contents, as well.

OptiFine does not have any restrictions either.

## Installation
Installation automatically occurs when launching OpenMantleProxy with `server`.

Full example: `dotnet Uranometrical.OpenMantleProxy.dll server --mode optimantle`

## Uninstallation
Run OpenMantleProxy with the `clean` option (instead of `server`) (`dotnet Uranometrical.OpenMantleProxy.dll clean`) to scrub away any residue left by Mantle or OpenMantleProxy (removes any redirects from `s.optifine.net`).

## Building
Simply clone the repository:
```sh
$ git clone https://github.com/uranometrical/openmantleproxy
```

Move into the `./src` directory:
```sh
$ cd openmantleproxy/src
```

And build the project:
```sh
$ dotnet build Uranometrical.OpenMantleProxy.sln -c Release
```
