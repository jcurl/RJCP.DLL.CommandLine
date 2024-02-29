namespace RJCP.Core.Terminal
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Resources;

    // Use type aliasing to ensure a more restrictive type in newer .NET versions. Improves code safety. In client code, use
    // `var` to automatically get the type.
#if NET45_OR_GREATER || NET6_0_OR_GREATER
    using IList = System.Collections.Generic.IReadOnlyList<string>;
#else
    using IList = System.Collections.Generic.IList<string>;
#endif

    internal abstract class Format
    {
        private const int MinimumWidth = 10;

        private enum TokenType
        {
            None,
            Word,
            Space,
            Newline
        }

        private struct Token
        {
            public TokenType Type;
            public string Word;
        }

        /// <summary>
        /// Gets the width of the terminal.
        /// </summary>
        /// <value>The width of the terminal.</value>
        public abstract int Width { get; }

        /// <summary>
        /// Gets the height of the terminal.
        /// </summary>
        /// <value>The height of the terminal.</value>
        public abstract int Height { get; }

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="line">The line to write to the terminal.</param>
        /// <returns>The formatted, wrapped lines.</returns>
        public IList WrapLine(string line)
        {
            return WrapLine(0, 0, line);
        }

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="indent">The first indent, in spaces, from the left of the terminal.</param>
        /// <param name="line">The line to write to the terminal.</param>
        /// <returns>The formatted, wrapped lines.</returns>
        public IList WrapLine(int indent, string line)
        {
            return WrapLine(indent, 0, line);
        }

        private static readonly string[] EmptyLine = new string[] { string.Empty };

        /// <summary>
        /// Writes the specified line to the terminal wrapped to the terminal width, with a newline character at the
        /// end.
        /// </summary>
        /// <param name="indent">The first indent, in spaces, from the left of the terminal.</param>
        /// <param name="hang">The hanging indent, relative to the <paramref name="indent"/>.</param>
        /// <param name="line">The line to write to the terminal.</param>
        /// <returns>The formatted, wrapped lines.</returns>
        public IList WrapLine(int indent, int hang, string line)
        {
            ThrowHelper.ThrowIfNegative(indent);
            int leftHang = indent + hang;
            if (leftHang < 0)
                throw new ArgumentOutOfRangeException(nameof(hang), CmdLineStrings.TerminalHangArgOutOfRange);

            // Special case - empty line.
            if (string.IsNullOrEmpty(line))
                return EmptyLine;

            if (Width <= MinimumWidth) {
                // The console width is very narrow, so push everything to the right.
                indent = 0;
                if (hang > 2) {
                    leftHang = 2;
                } else {
                    leftHang = Math.Max(0, indent + hang);
                }
            } else if (indent > Width - MinimumWidth) {
                // We leave that the indent doesn't exceed the MinimumWidth on the right.
                int shift = indent - (Width - MinimumWidth);
                indent -= shift;
                leftHang -= shift;
                if (leftHang < 0) leftHang = 0;
                if (leftHang > Width - MinimumWidth + 2)
                    leftHang = Width - MinimumWidth + 2;
            }

            StringBuilder sbLine = new(Width);
            List<string> lines = new();

            int lineIndent = indent;
            bool newLine = true;
            int offset = 0;
            while (offset < line.Length) {
                Token token = GetWord(line, ref offset);
                switch (token.Type) {
                case TokenType.None:
                    // Should never occur
                    break;
                case TokenType.Space:
                    // If the first token is a space, then we take this as a hanging indent.
                    lineIndent = leftHang;
                    break;
                case TokenType.Newline:
                    if (newLine) {
                        lines.Add(string.Empty);
                    } else {
                        lines.Add(sbLine.ToString());
                        sbLine.Clear();
                        newLine = true;
                    }
                    lineIndent = indent;
                    break;
                case TokenType.Word:
                    if (newLine) {
                        sbLine.Append(' ', lineIndent).Append(token.Word);
                        lineIndent = leftHang;
                    } else {
                        if (sbLine.Length + token.Word.Length + 1 >= Width) {
                            lines.Add(sbLine.ToString());
                            sbLine.Clear();
                            sbLine.Append(' ', lineIndent).Append(token.Word);
                        } else {
                            sbLine.Append(' ').Append(token.Word);
                        }
                    }
                    newLine = false;
                    break;
                default:
                    // Should never occur
                    throw new InvalidOperationException(CmdLineStrings.TerminalParseError);
                }
            }

            lines.Add(sbLine.ToString());
            return lines;
        }

        private static Token GetWord(string line, ref int offset)
        {
            Token token = new() {
                Type = TokenType.None,
                Word = null
            };

            for (int i = offset; i < line.Length; i++) {
                char c = line[i];

                switch (token.Type) {
                case TokenType.None:
                    if (c == '\n') {
                        offset = i + 1;
                        token.Type = TokenType.Newline;
                        return token;
                    } else if (c == '\r') {
                        token.Type = TokenType.Newline;
                    } else if (char.IsWhiteSpace(c) || char.IsControl(c) || char.IsSeparator(c)) {
                        token.Type = TokenType.Space;
                    } else {
                        token.Type = TokenType.Word;
                    }
                    break;
                case TokenType.Newline:
                    if (c == '\n') {
                        offset = i + 1;
                        return token;
                    }
                    offset = i;
                    return token;
                case TokenType.Space:
                    if (!char.IsWhiteSpace(c) && !char.IsControl(c) && !char.IsSeparator(c)) {
                        offset = i;
                        return token;
                    }
                    break;
                case TokenType.Word:
                    if (char.IsWhiteSpace(c) || char.IsControl(c) || char.IsSeparator(c) || c == '\r' || c == '\n') {
                        token.Word =
#if NET6_0_OR_GREATER
                            line[offset..i];
#else
                            line.Substring(offset, i - offset);
#endif
                        offset = i;
                        return token;
                    }
                    break;
                default:
                    // Should never occur
                    throw new InvalidOperationException(CmdLineStrings.TerminalParseError);
                }
            }

            switch (token.Type) {
            case TokenType.None:
            case TokenType.Newline:
            case TokenType.Space:
                offset = line.Length;
                return token;
            case TokenType.Word:
                token.Word =
#if NET6_0_OR_GREATER
                    line[offset..];
#else
                    line.Substring(offset);
#endif
                offset = line.Length;
                return token;
            default:
                // Should never occur
                throw new InvalidOperationException(CmdLineStrings.TerminalParseError);
            }
        }
    }
}
