namespace RJCP.Core.Terminal
{
    /// <summary>
    /// The ITerminalOut handles writing to the terminal.
    /// </summary>
    public interface ITerminalOut
    {
        /// <summary>
        /// Writes the specified line to the terminal.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        void Write(string line);

        /// <summary>
        /// Writes the specified line to the terminal.
        /// </summary>
        /// <param name="format">The format line to write to the terminal.</param>
        /// <param name="args">The arguments to be formatted.</param>
        void Write(string format, params object[] args);

        /// <summary>
        /// Writes an empty line to the terminal with a newline character at the end.
        /// </summary>
        void WriteLine();

        /// <summary>
        /// Writes the specified line to the terminal with a newline character at the end.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        void WriteLine(string line);

        /// <summary>
        /// Writes the specified line to the terminal with a newline character at the end.
        /// </summary>
        /// <param name="format">The format line to write to the terminal.</param>
        /// <param name="args">The arguments to be formatted.</param>
        void WriteLine(string format, params object[] args);

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        void WrapLine(string line);

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="format">The format line to write to the terminal.</param>
        /// <param name="args">The arguments to be formatted.</param>
        void WrapLine(string format, params object[] args);

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="indent">The first indent, in spaces, from the left of the terminal.</param>
        /// <param name="line">The line to write to the terminal.</param>
        void WrapLine(int indent, string line);

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="indent">The first indent, in spaces, from the left of the terminal.</param>
        /// <param name="format">The format line to write to the terminal.</param>
        /// <param name="args">The arguments to be formatted.</param>
        void WrapLine(int indent, string format, params object[] args);

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="indent">The first indent, in spaces, from the left of the terminal.</param>
        /// <param name="hang">The hanging indent, relative to the <paramref name="indent"/>.</param>
        /// <param name="line">The line to write to the terminal.</param>
        void WrapLine(int indent, int hang, string line);

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="indent">The first indent, in spaces, from the left of the terminal.</param>
        /// <param name="hang">The hanging indent, relative to the <paramref name="indent"/>.</param>
        /// <param name="format">The format line to write to the terminal.</param>
        /// <param name="args">The arguments to be formatted.</param>
        void WrapLine(int indent, int hang, string format, params object[] args);
    }
}
