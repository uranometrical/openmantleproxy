using System;
using System.Collections.Generic;
using System.IO;

namespace Uranometrical.OpenMantleProxy.Hosts
{
    /// <summary>
    ///     An object that handles editing a system's <c>hosts</c> file.
    /// </summary>
    public class HostsFileEditor
    {
        /// <summary>
        ///     An <see cref="Action"/> (<see cref="Delegate"/>) allowing a user to interface with <see cref="HostsFileEditor"/> to enable file writing.
        /// </summary>
        public delegate IEnumerable<string> HostsFileWriter(string[] contents);

        protected readonly FileInfo HostsFile;
        protected readonly string[] Contents;

        /// <summary>
        ///     Creates a new <see cref="HostsFileEditor"/> instance.
        /// </summary>
        /// <param name="hostsFile">The <see cref="FileInfo"/> pointing to the system's <c>hosts</c> file.</param>
        public HostsFileEditor(FileInfo hostsFile)
        {
            HostsFile = hostsFile;
            Contents = File.ReadAllLines(HostsFile.ToString());
        }

        /// <summary>
        ///     Creates a new <see cref="HostsFileEditor"/> instance.
        /// </summary>
        /// <param name="hostsFile">A <see cref="string"/> input which is used to construct a <see cref="FileInfo"/>.</param>
        public HostsFileEditor(string hostsFile) : this(new FileInfo(hostsFile))
        {
        }

        /// <summary>
        ///     Creates a new <see cref="HostsFileEditor"/> instance which automatically resolves a <c>hosts</c> file's location.
        /// </summary>
        public HostsFileEditor() : this(HostsFileResolver.ResolveBlocking())
        {
        }

        /// <summary>
        ///     Handles writing to the <c>hosts</c> file.
        /// </summary>
        /// <param name="fileWriter"></param>
        public virtual void Write(HostsFileWriter fileWriter) => File.WriteAllLines(
            HostsFile.ToString(),
            fileWriter(Contents)
        );
    }
}