namespace RJCP.Core.Terminal.Log
{
    using System;

    /// <summary>
    /// Event Arguments when a line is written to the console.
    /// </summary>
    public sealed class TerminalWriteEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TerminalWriteEventArgs"/> class.
        /// </summary>
        /// <param name="channel">The channel that the log line was written to.</param>
        /// <param name="newLine">If this ended with a newline character.</param>
        /// <param name="line">The line that was written.</param>
        public TerminalWriteEventArgs(ConsoleLogChannel channel, bool newLine, string line)
        {
            Channel = channel;
            NewLine = newLine;
            Line = line;
        }

        /// <summary>
        /// Gets the channel that the log line was written to.
        /// </summary>
        /// <value>The channel that the log line was written to.</value>
        public ConsoleLogChannel Channel { get; }

        /// <summary>
        /// Indicates if this ended with a newline character.
        /// </summary>
        /// <value>Is <see langword="true"/> if this ends with a new line; otherwise, <see langword="false"/>.</value>
        public bool NewLine { get; }

        /// <summary>
        /// Gets the line that was written.
        /// </summary>
        /// <value>The line that was written. May be a partial line (see <see cref="NewLine"/>).</value>
        public string Line { get; }
    }
}
