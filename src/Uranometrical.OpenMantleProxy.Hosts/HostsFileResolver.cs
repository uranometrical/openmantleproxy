using System;
using System.IO;
using Microsoft.Win32;

namespace Uranometrical.OpenMantleProxy.Hosts
{
    /// <summary>
    ///     Resolves the location of a computer's <c>hosts</c> file.
    /// </summary>
    public static class HostsFileResolver
    {
        /// <summary>
        ///     Identical to <see cref="Resolve"/>, but requires user input in the event of a failed resolution.
        /// </summary>
        /// <returns>A <see cref="FileInfo"/> pointing to the <c>hosts</c> file.</returns>
        public static FileInfo ResolveBlocking()
        {
            FileInfo file = Resolve();

            while (!file.Exists)
            {
                Console.WriteLine("Could not resolve hosts file, please input the location manually:");
                file = new FileInfo(Console.ReadLine() ?? "");
            }

            return file;
        }

        /// <summary>
        ///     Resolves the default <c>hosts</c> file location on *Nix and Windows.
        /// </summary>
        /// <returns>A <see cref="FileInfo"/> pointing to the <c>hosts</c> file.</returns>
        public static FileInfo Resolve() =>
            Environment.OSVersion.Platform == PlatformID.Unix ? ResolveUnix() : ResolveWindows();

        /// <summary>
        ///     Resolves the <c>hosts</c> file location on Windows systems.
        /// </summary>
        /// <returns>A <see cref="FileInfo"/> pointing to the <c>hosts</c> file.</returns>
        public static FileInfo ResolveWindows()
        {
            // https://github.com/uranometrical/openmantleproxy/issues/1
            return new FileInfo((string) Registry.GetValue(
                @"\HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Tcpip\Parameters",
                "DataBasePath",
                @"C:\Windows\System32\Drivers\etc\hosts"
            )!);
        }

        /// <summary>
        ///     Resolves the <c>hosts</c> file location on *Nix systems.
        /// </summary>
        /// <returns>A <see cref="FileInfo"/> pointing to the <c>hosts</c> file.</returns>
        public static FileInfo ResolveUnix() => new("/etc/hosts");
    }
}