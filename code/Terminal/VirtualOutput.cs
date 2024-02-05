namespace RJCP.Core.Terminal
{
    using System.Collections.Generic;

    /// <summary>
    /// Writes the output to the virtual console.
    /// </summary>
    internal class VirtualOutput : StdOutBase
    {
        private enum LineState
        {
            None,
            CarriageReturn
        }

        private bool m_NewLine = true;
        private LineState m_LineState;
        private readonly List<string> m_Lines = new List<string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="VirtualOutput"/> class.
        /// </summary>
        /// <param name="format">The class responsible for formatting lines.</param>
        public VirtualOutput(Format format) : base(format) { }

        /// <summary>
        /// Gets the lines that were written.
        /// </summary>
        /// <value>The lines that were written.</value>
        /// <remarks>
        /// Since this is expected to be used only for the purpose of testing, populating the <see cref="Lines"/>
        /// collection is not expected to generate any high memory usage during unit testing.
        /// </remarks>
        public virtual List<string> Lines { get { return m_Lines; } }

        /// <summary>
        /// Writes the specified line to the terminal.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        public override void Write(string line)
        {
            if (line != null)
                SplitLines(line);
        }

        /// <summary>
        /// Writes the specified line to the terminal with a newline character at the end.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        public override void WriteLine(string line)
        {
            if (line != null)
                SplitLines(line);
            NewLine();
        }

        private static readonly char[] NewLines = new[] { '\r', '\n' };

        private void SplitLines(string line)
        {
            int offset = 0;
            int length = line.Length;
            while (offset < length) {
                int charPos = line.IndexOfAny(NewLines, offset);
                if (charPos == -1) {
                    AddLine(line, offset, length - offset);
                    m_LineState = LineState.None;
                    m_NewLine = false;
                    return;
                } else {
                    int c = line[charPos];
                    if (m_LineState == LineState.CarriageReturn && charPos == offset && c == '\n') {
                        // If a \r is followed immediately by a \n, then ignore it, as this is the Windows new line
                        // sequence, the CR LF sequence (\r\n).
                        m_LineState = LineState.None;
                    } else {
                        AddLine(line, offset, charPos - offset);
                        m_NewLine = true;
                        m_LineState = (c == '\r') ? LineState.CarriageReturn : LineState.None;
                    }
                    offset = charPos + 1;
                }
            }
        }

        private void AddLine(string line, int offset, int length)
        {
#if NET6_0_OR_GREATER
            string lineSpan = line[offset..(offset + length)];
            if (m_NewLine) {
                m_Lines.Add(lineSpan);
            } else {
                m_Lines[^1] += lineSpan;
            }
#else
            if (m_NewLine) {
                m_Lines.Add(line.Substring(offset, length));
            } else {
                m_Lines[m_Lines.Count - 1] += line.Substring(offset, length);
            }
#endif
        }

        private void NewLine()
        {
            if (m_NewLine) {
                m_Lines.Add(string.Empty);
            }
            m_LineState = LineState.None;
            m_NewLine = true;
        }
    }
}
