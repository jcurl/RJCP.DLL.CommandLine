namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections.Generic;
    using Resources;

    /// <summary>
    /// Interpret arguments similar to GNU long options.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Arguments are options if they begin with a hyphen delimiter (‘-’).</item>
    /// <item>
    /// Multiple options may follow a hyphen delimiter in a single token if the options do not take arguments. Thus,
    /// <c>-abc</c> is equivalent to <c>-a -b -c</c>.
    /// </item>
    /// <item>
    /// Option names are single alphanumeric characters. This is defined by the <see cref="OptionAttribute"/>.
    /// </item>
    /// <item>Certain options require an argument.</item>
    /// <item>
    /// An option and its argument may or may not appear as separate tokens. (In other words, the whitespace separating
    /// them is optional.) Thus, <c>-o</c> foo and <c>-ofoo</c> are equivalent.
    /// </item>
    /// <item>The argument <c>--</c> terminates all options</item>
    /// <item>
    /// A token consisting of a single hyphen character <c>-</c> is interpreted as an ordinary non-option argument. By
    /// convention, it is used to specify input from or output to the standard input and output streams.
    /// </item>
    /// <item>
    /// Options may be supplied in any order, or appear multiple times. The interpretation is left up to the particular
    /// application program.
    /// </item>
    /// <item>
    /// Long options consist of <c>--</c> followed by a name made of alphanumeric characters and dashes. We don't allow
    /// abbreviations in this implementation.
    /// </item>
    /// <item>
    /// To specify an argument for a long option, write <c>--name=value</c>. This implementation allows spaces also.
    /// </item>
    /// </list>
    /// </remarks>
    internal sealed class UnixOptionParser : IOptionParser
    {
        private string[] m_Arguments;

        public OptionsStyle Style { get { return OptionsStyle.Unix; } }

        public bool LongOptionCaseInsensitive { get { return true; } }

        public char ListSeparator { get { return ','; } }

        public string ShortOptionPrefix { get { return "-"; } }

        public string LongOptionPrefix { get { return "--"; } }

        public string AssignmentSymbol { get { return "="; } }

        private readonly Queue<OptionToken> m_Tokens = new Queue<OptionToken>();
        private readonly Queue<OptionToken> m_NonOptArgs = new Queue<OptionToken>();
        private bool m_ArgumentsOnly;
        private int m_ArgumentPosition;
        private int m_ArgumentCharPosition = -1;

        public void AddArguments(string[] arguments)
        {
            if (m_Arguments != null)
                throw new InvalidOperationException();
            m_Arguments = arguments;
        }

        public OptionToken GetToken(bool expectValue)
        {
            if (m_Arguments == null) return null;

            bool haveTokens = true;
            while (haveTokens && m_Tokens.Count == 0) {
                haveTokens = ParseNextArgument(expectValue);
            }

            if (m_Tokens.Count > 0) {
                // If expecting a value, return only a value. If we're not expecting a value, the implementation
                // of `ParseNextArgument` should not return a value, but an argument.
                if (expectValue && m_Tokens.Peek().Token != OptionTokenKind.Value)
                    return null;
                return m_Tokens.Dequeue();
            }
            if (expectValue) return null;

            if (m_NonOptArgs.Count > 0) {
                return m_NonOptArgs.Dequeue();
            }

            return null;
        }

        /// <summary>
        /// Parses the next argument.
        /// </summary>
        /// <param name="expectValue">Expect a value to an argument if set to <see langword="true"/>.</param>
        /// <returns>
        /// Returns <see langword="true"/> if an argument was parsed, <see langword="false"/> otherwise if there are no
        /// more arguments.
        /// </returns>
        /// <exception cref="OptionException">Error parsing short options.</exception>
        /// <remarks>
        /// Parses the current argument. It may put it in <c>m_Tokens</c> or <c>m_NonOptArgs</c>. The non-option
        /// arguments should be parsed only when all tokens in <c>m_Tokens</c> have been parsed.
        /// </remarks>
        private bool ParseNextArgument(bool expectValue)
        {
            if (m_ArgumentPosition >= m_Arguments.Length) return false;

            string argument = m_Arguments[m_ArgumentPosition];

            if (m_ArgumentsOnly) {
                m_NonOptArgs.Enqueue(new OptionToken(OptionTokenKind.Argument, argument));
                m_ArgumentPosition++;
                return true;
            }

            if (m_ArgumentCharPosition != -1) {
                // Continue parsing the short options.
                if (expectValue) {
                    // All remaining characters are actually the value for the previous short option.
                    m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value,
                        argument.Substring(m_ArgumentCharPosition)));
                    m_ArgumentCharPosition = -1;
                    m_ArgumentPosition++;
                    return true;
                }
                m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption,
                    argument[m_ArgumentCharPosition].ToString()));
                m_ArgumentCharPosition++;
                if (argument.Length == m_ArgumentCharPosition) {
                    m_ArgumentCharPosition = -1;
                    m_ArgumentPosition++;
                }
                return true;
            }

            if (argument.Length > 0 && argument[0] == '-') {
                if (argument.Length > 1 && argument[1] == '-') {
                    // This is a long option, starts with --
                    if (argument.Length > 2) {
                        int equal = argument.IndexOf('=', 2);
                        if (equal == -1) {
                            m_Tokens.Enqueue(new OptionToken(OptionTokenKind.LongOption, argument.Substring(2)));
                            m_ArgumentPosition++;
                            return true;
                        }
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.LongOption, argument.Substring(2, equal - 2)));
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value, argument.Substring(equal + 1)));
                        m_ArgumentPosition++;
                        return true;
                    }

                    // The string -- occurs alone, don't parse any further options. They're all arguments. If this is
                    // given after an option expecting a value, we assume that there were no parameters given to that
                    // option,
                    m_ArgumentsOnly = true;
                    m_ArgumentPosition++;
                    return true;
                }

                // This is a short option, starts with -
                if (argument.Length > 1) {
                    int equal = argument.IndexOf('=', 1);
                    if (equal != -1) {
                        if (equal != 2) {
                            string message = string.Format(CmdLineStrings.UnixCmdLineToken, argument);
                            throw new OptionException(message);
                        }
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption, argument[1].ToString()));
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value, argument.Substring(3)));
                        m_ArgumentPosition++;
                        return true;
                    }

                    if (argument.Length > 2) {
                        // There are multiple short options. We've just parsed the very first.
                        m_ArgumentCharPosition = 2;
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption, argument[1].ToString()));
                        return true;
                    }

                    m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption, argument[1].ToString()));
                    m_ArgumentPosition++;
                    return true;
                }

                // A - alone, is just an argument (e.g. by convention, stdin).
                if (expectValue) {
                    m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value, argument));
                } else {
                    m_NonOptArgs.Enqueue(new OptionToken(OptionTokenKind.Argument, argument));
                }
                m_ArgumentPosition++;
                return true;
            }

            // It's not an option, so this is just an argument or the value for the previous option.
            if (expectValue) {
                m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value, argument));
            } else {
                m_NonOptArgs.Enqueue(new OptionToken(OptionTokenKind.Argument, argument));
            }
            m_ArgumentPosition++;
            return true;
        }
    }
}
