# OpenMantleProxy
Open-source alternative to the traditional Mantle program.

## Functionality
When ran, OpenMantleProxy starts a simple local HTTP server on an available port. It then modifies your `hosts` file to direct `s.optifine.net` requests to the newly-started server (this requires administrator privileges on Windows and root on *Nix).

Now that the OpenMantleProxy server can actively receive OptiFine requests, it can reroute them as needed.

### Modes
There are three different proxy modes in OpenMantleProxy, detailed below:

#### `mantle`
`mantle` is the default mode, and reroutes all `s.optifine.net` requests to `capes.mantle.gg`. This is the same behavior as regular Mantle.

#### `optimantle`
`optimantle` initially seends an HTTP request to `s.optifine.net`. If a cape is not resolved, then an HTTP request is sent to `capes.mantle.gg`.

#### `mantlefine`
`mantlefine`, like `optimantle`, handles sending HTTP requests to `s.optifine.net` and `capes.mantle.gg`, but in reversed order.
