namespace RJCP.Core.Terminal
{
    using System;

    /// <summary>
    /// A <see cref="ITerminal"/> class that writes to the console.
    /// </summary>
    public sealed class ConsoleTerminal : ITerminal
    {
        private readonly ConsoleFormat m_Format = new ConsoleFormat();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleTerminal"/> class.
        /// </summary>
        public ConsoleTerminal()
        {
            StdOut = new StdOut(m_Format);
            StdErr = new StdErr(m_Format);
        }

        /// <summary>
        /// Gets the standard out terminal handle.
        /// </summary>
        /// <value>The standard out terminal handle.</value>
        public ITerminalOut StdOut { get; }

        /// <summary>
        /// Gets the standard error terminal handle.
        /// </summary>
        /// <value>The standard error terminal handle.</value>
        public ITerminalOut StdErr { get; }

        /// <summary>
        /// Gets the width of the terminal.
        /// </summary>
        /// <value>The width of the terminal.</value>
        public int Width { get { return m_Format.Width; } }

        /// <summary>
        /// Gets the height of the terminal.
        /// </summary>
        /// <value>The height of the terminal.</value>
        public int Height { get { return m_Format.Height; } }

        private ConsoleColor m_ForegroundColorShadow = ConsoleColor.Gray;

        /// <summary>
        /// Gets or sets the terminal foreground colour.
        /// </summary>
        /// <value>The color of the foreground.</value>
        public ConsoleColor ForegroundColor
        {
            get
            {
                if (m_Format.IsRedirected)
                    return m_ForegroundColorShadow;
                return Console.ForegroundColor;
            }
            set
            {
                if (m_Format.IsRedirected) {
                    m_ForegroundColorShadow = value;
                } else {
                    Console.ForegroundColor = value;
                }
            }
        }

        private ConsoleColor m_BackgroundColorShadow = ConsoleColor.Black;

        /// <summary>
        /// Gets or sets the terminal background colour.
        /// </summary>
        /// <value>The color of the background.</value>
        public ConsoleColor BackgroundColor
        {
            get
            {
                if (m_Format.IsRedirected)
                    return m_BackgroundColorShadow;
                return Console.BackgroundColor;
            }
            set
            {
                if (m_Format.IsRedirected) {
                    m_BackgroundColorShadow = value;
                } else {
                    Console.BackgroundColor = value;
                }
            }
        }
    }
}
