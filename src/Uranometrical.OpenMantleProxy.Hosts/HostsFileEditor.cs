using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Uranometrical.OpenMantleProxy.Hosts
{
    /// <summary>
    ///     Disposable object that handles editing a system's <c>hosts</c> file.
    /// </summary>
    public class HostsFileEditor : IDisposable
    {
        /// <summary>
        ///     An <see cref="Action"/> (<see cref="Delegate"/>) allowing a user to interface with <see cref="HostsFileEditor"/> to enable file writing.
        /// </summary>
        public delegate void HostsFileWriter(StreamWriter writer, string[] contents);

        protected readonly FileInfo HostsFile;
        protected readonly FileStream HostsStream;
        protected readonly StreamWriter HostsWriter;
        protected readonly string[] Contents;

        /// <summary>
        ///     Creates a new <see cref="HostsFileEditor"/> instance.
        /// </summary>
        /// <param name="hostsFile">The <see cref="FileInfo"/> pointing to the system's <c>hosts</c> file.</param>
        public HostsFileEditor(FileInfo hostsFile)
        {
            HostsFile = hostsFile;
            HostsStream = new FileStream(HostsFile.ToString(), FileMode.Open, FileAccess.ReadWrite);
            HostsWriter = new StreamWriter(HostsStream)
            {
                AutoFlush = true
            };

            string? line;
            List<string> lines = new();
            using StreamReader reader = new(HostsStream, Encoding.UTF8, true, 1024, true);

            while ((line = reader.ReadLine()) != null)
                lines.Add(line);

            Contents = lines.ToArray();
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
        public virtual void Write(HostsFileWriter fileWriter) => fileWriter(HostsWriter, Contents);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            HostsWriter.Dispose();
            HostsStream.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}