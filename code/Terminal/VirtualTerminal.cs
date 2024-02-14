namespace RJCP.Core.Terminal
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A <see cref="ITerminal"/> class that writes to a virtualised console.
    /// </summary>
    public sealed class VirtualTerminal : ITerminal
    {
        private readonly VirtualFormat m_Format = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualTerminal"/> class.
        /// </summary>
        public VirtualTerminal()
        {
            VirtualOutput stdOut = new(m_Format);
            VirtualOutput stdErr = new(m_Format);
            StdOut = stdOut;
            StdErr = stdErr;
            StdOutLines = new ReadOnlyCollection<string>(stdOut.Lines);
            StdErrLines = new ReadOnlyCollection<string>(stdErr.Lines);
        }

        /// <summary>
        /// Gets the standard out terminal handle.
        /// </summary>
        /// <value>The standard out terminal handle.</value>
        public ITerminalOut StdOut { get; }

        /// <summary>
        /// Gets the lines that were written to the standard out.
        /// </summary>
        /// <value>The lines written to the standard out.</value>
        public IList<string> StdOutLines { get; }

        /// <summary>
        /// Gets the standard error terminal handle.
        /// </summary>
        /// <value>The standard error terminal handle.</value>
        public ITerminalOut StdErr { get; }

        /// <summary>
        /// Gets the lines that were written to the standard error.
        /// </summary>
        /// <value>The lines written to the standard error.</value>
        public IList<string> StdErrLines { get; }

        /// <summary>
        /// Gets the width of the terminal.
        /// </summary>
        /// <value>The width of the terminal.</value>
        public int Width
        {
            get { return m_Format.Width; }
            set { m_Format.SetWidth(value); }
        }

        /// <summary>
        /// Gets the height of the terminal.
        /// </summary>
        /// <value>The height of the terminal.</value>
        public int Height
        {
            get { return m_Format.Height; }
            set { m_Format.SetHeight(value); }
        }

        /// <summary>
        /// Gets or sets the terminal foreground colour.
        /// </summary>
        /// <value>The color of the foreground.</value>
        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Gray;

        /// <summary>
        /// Gets or sets the terminal background colour.
        /// </summary>
        /// <value>The color of the background.</value>
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
    }
}
