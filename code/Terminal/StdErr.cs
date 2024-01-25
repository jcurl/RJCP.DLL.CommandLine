namespace RJCP.Core.Terminal
{
    using System;

    /// <summary>
    /// Writes the standard error to the console.
    /// </summary>
    internal class StdErr : StdOutBase
    {
        public StdErr(Format format) : base(format) { }

        /// <summary>
        /// Writes the specified line to the terminal.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        public override void Write(string line)
        {
            Console.Error.Write(line);
        }

        /// <summary>
        /// Writes the specified line to the terminal with a newline character at the end.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        public override void WriteLine(string line)
        {
            Console.Error.WriteLine(line);
        }
    }
}
