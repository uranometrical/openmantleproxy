# OpenMantleProxy
Open-source alternative to the traditional Mantle program.

## Functionality
OpenMantleProxy starts a local HTTP server to process any received requests. After starting the HTTP server, OpenMantleProxy modifies the computer's `hosts` file (the same way Mantle does) to redirect any `s.optifine.net` (OptiFine cape server) requests. Different modes will make the redirect function differently.

Due to the requirement of modifying the computer's `hosts` file, it is more than likely that the program will require administrator privileges on any Windows operating system, and root on *Nix.

To start a server, run `dotnet Uranometrical.OpenMantleProxy.dll server`. Please read the "Modes" section to understand which one is best for you.

### Modes
Use a mode by appending `--mode` (optionally, `-m`) to the command above, followed by the mode's name.

There are three different proxy modes in OpenMantleProxy, detailed below:

#### `mantle`
`mantle` is the default mode, and reroutes all `s.optifine.net` requests to `capes.mantle.gg`. This is the same behavior as regular Mantle.

#### `optimantle`
`optimantle` initially sends an HTTP request to `s.optifine.net`. If a cape is not resolved, then an HTTP request is sent to `capes.mantle.gg`.

#### `mantlefine`
`mantlefine` functions identically to `optifine`, just in reverse.

#### `custom`
`custom` ignores any predefined servers and instead expects an additional input (`--servers`/`-s`), which it will pull from for capes instead.

## Security
OpenMantleProxy only establishes connections (conditionally) to `capes.mantle.gg` and `s.optifine.net`. As long as these domains remain safe moving forward (essentially assuming no malicious third party takes control of either domain), there are no security risks. Always be safe while installing software online, however, as this program does require elevated permissions. If you do not feel a binary release is safe, please compile this program manually after throughly examining its code.

## Safety
OpenMantleProxy can be used with any Minecraft client that uses OptiFine without fear of breaking the client's Terms of Service. Clients such as Lunar like to claim that Mantle violates their terms, but it doesn't, as Mantle does not touch Lunar. It modifies core OS functionalities. This is the same for OpenMantleProxy.

Mantle's terms do not prevent third-party programs from accessing their website or its contents, as well.

OptiFine does not have any restrictions either.

## Installation
Installation automatically occurs when launching OpenMantleProxy without `--clean` (`-c`). Modes will not affect how the instllation is done.

## Uninstallation
Run OpenMantleProxy with the `clean` option (instead of `server`) to scrub away any residue left by Mantle or OpenMantleProxy (removes any redirects from `s.optifine.net`).
