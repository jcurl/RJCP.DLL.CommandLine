namespace RJCP.Core.Terminal
{
    using System;
    using Log;

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
            OnWriteEvent(this, new TerminalWriteEventArgs(ConsoleLogChannel.StdErr, false, line));
            Console.Error.Write(line);
        }

        /// <summary>
        /// Writes the specified line to the terminal with a newline character at the end.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        public override void WriteLine(string line)
        {
            OnWriteEvent(this, new TerminalWriteEventArgs(ConsoleLogChannel.StdErr, true, line));
            Console.Error.WriteLine(line);
        }

        /// <summary>
        /// Get a notification when a line to this console channel is written.
        /// </summary>
        public event EventHandler<TerminalWriteEventArgs> ConsoleWriteEvent;

        private void OnWriteEvent(object sender, TerminalWriteEventArgs eventArgs)
        {
            EventHandler<TerminalWriteEventArgs> handler = ConsoleWriteEvent;
            if (handler != null) handler(sender, eventArgs);
        }
    }
}
