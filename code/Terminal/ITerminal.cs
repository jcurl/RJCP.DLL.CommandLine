namespace RJCP.Core.Terminal
{
    using System;

    /// <summary>
    /// ITerminal represents a terminal device which can be written to.
    /// </summary>
    public interface ITerminal
    {
        /// <summary>
        /// Gets the standard out terminal handle.
        /// </summary>
        /// <value>The standard out terminal handle.</value>
        ITerminalOut StdOut { get; }

        /// <summary>
        /// Gets the standard error terminal handle.
        /// </summary>
        /// <value>The standard error terminal handle.</value>
        ITerminalOut StdErr { get; }

        /// <summary>
        /// Gets the width of the terminal.
        /// </summary>
        /// <value>The width of the terminal.</value>
        int Width { get; }

        /// <summary>
        /// Gets the height of the terminal.
        /// </summary>
        /// <value>The height of the terminal.</value>
        int Height { get; }

        /// <summary>
        /// Gets or sets the terminal foreground colour.
        /// </summary>
        /// <value>The color of the foreground.</value>
        ConsoleColor ForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the terminal background colour.
        /// </summary>
        /// <value>The color of the background.</value>
        ConsoleColor BackgroundColor { get; set; }
    }
}
