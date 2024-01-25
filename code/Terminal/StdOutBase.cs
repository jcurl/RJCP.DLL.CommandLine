namespace RJCP.Core.Terminal
{
    /// <summary>
    /// A base implementation for other classes to write to the console.
    /// </summary>
    internal abstract class StdOutBase : ITerminalOut
    {
        private readonly Format m_Format;

        protected StdOutBase(Format format)
        {
            m_Format = format;
        }

        /// <summary>
        /// Writes the specified line to the terminal.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        public abstract void Write(string line);

        /// <summary>
        /// Writes the specified line to the terminal with a newline character at the end.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        public abstract void WriteLine(string line);

        /// <summary>
        /// Writes the specified line to the terminal.
        /// </summary>
        /// <param name="format">The format line to write to the terminal.</param>
        /// <param name="args">The arguments to be formatted.</param>
        public void Write(string format, params object[] args)
        {
            string line = string.Format(format, args);
            Write(line);
        }

        /// <summary>
        /// Writes the specified line to the terminal with a newline character at the end.
        /// </summary>
        /// <param name="format">The format line to write to the terminal.</param>
        /// <param name="args">The arguments to be formatted.</param>
        public void WriteLine(string format, params object[] args)
        {
            string line = string.Format(format, args);
            WriteLine(line);
        }

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        public void WrapLine(string line)
        {
            var list = m_Format.WrapLine(line);
            foreach (string consoleLine in list) {
                WriteLine(consoleLine);
            }
        }

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="format">The format line to write to the terminal.</param>
        /// <param name="args">The arguments to be formatted.</param>
        public void WrapLine(string format, params object[] args)
        {
            string line = string.Format(format, args);
            WrapLine(line);
        }

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="indent">The first indent, in spaces, from the left of the terminal.</param>
        /// <param name="line">The line to write to the terminal.</param>
        public void WrapLine(int indent, string line)
        {
            var list = m_Format.WrapLine(indent, line);
            foreach (string consoleLine in list) {
                WriteLine(consoleLine);
            }
        }

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="indent">The first indent, in spaces, from the left of the terminal.</param>
        /// <param name="format">The format line to write to the terminal.</param>
        /// <param name="args">The arguments to be formatted.</param>
        public void WrapLine(int indent, string format, params object[] args)
        {
            string line = string.Format(format, args);
            WrapLine(indent, line);
        }

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="indent">The first indent, in spaces, from the left of the terminal.</param>
        /// <param name="hang">The hanging indent, relative to the <paramref name="indent" />.</param>
        /// <param name="line">The line to write to the terminal.</param>
        public void WrapLine(int indent, int hang, string line)
        {
            var list = m_Format.WrapLine(indent, hang, line);
            foreach (string consoleLine in list) {
                WriteLine(consoleLine);
            }
        }

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="indent">The first indent, in spaces, from the left of the terminal.</param>
        /// <param name="hang">The hanging indent, relative to the <paramref name="indent" />.</param>
        /// <param name="format">The format line to write to the terminal.</param>
        /// <param name="args">The arguments to be formatted.</param>
        public void WrapLine(int indent, int hang, string format, params object[] args)
        {
            string line = string.Format(format, args);
            WrapLine(indent, hang, line);
        }
    }
}
