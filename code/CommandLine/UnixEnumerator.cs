namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal sealed class UnixOptionParser : IOptionParser
    {
        private string[] m_Arguments;
        private Queue<OptionToken> m_Tokens = new Queue<OptionToken>();

        public UnixOptionParser(string[] arguments)
        {
            m_Arguments = arguments;
        }

        public OptionToken GetToken(bool expectValue)
        {
            if (m_Arguments == null) return null;

            while (m_ArgumentPosition < m_Arguments.Length && m_Tokens.Count == 0) {
                ParseNextArgument(expectValue);
            }

            if (m_Tokens.Count > 0) {
                if (expectValue) {
                    OptionTokenKind tokenKind = m_Tokens.Peek().Token;
                    if (tokenKind == OptionTokenKind.Argument || tokenKind == OptionTokenKind.Value) {
                        OptionToken token = m_Tokens.Dequeue();
                        token.Token = OptionTokenKind.Value;
                        return token;
                    }
                    // Didn't find a value/argument, which is an error
                    return null;
                }
                return m_Tokens.Dequeue();
            }
            return null;
        }

        public bool LongOptionCaseInsenstive { get { return true; } }

        private bool m_ArgumentsOnly;
        private int m_ArgumentPosition;
        private int m_ArgumentCharPosition = -1;

        private void ParseNextArgument(bool expectValue)
        {
            string argument = m_Arguments[m_ArgumentPosition];

            if (m_ArgumentsOnly) {
                m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Argument, argument));
                m_ArgumentPosition++;
                return;
            }

            if (m_ArgumentCharPosition != -1) {
                // Continue parsing the short options
                if (expectValue) {
                    // All remaining characters are actually the value for the previous short option
                    m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value,
                        argument.Substring(m_ArgumentCharPosition)));
                    m_ArgumentCharPosition = -1;
                    m_ArgumentPosition++;
                    return;
                }
                m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption,
                    argument[m_ArgumentCharPosition].ToString()));
                m_ArgumentCharPosition++;
                if (argument.Length == m_ArgumentCharPosition) {
                    m_ArgumentCharPosition = -1;
                    m_ArgumentPosition++;
                }
                return;
            }

            if (argument.Length > 0 && argument[0] == '-') {
                if (argument.Length > 1 && argument[1] == '-') {
                    // This is a long option, starts with --
                    if (argument.Length > 2) {
                        int equal = argument.IndexOf('=', 2);
                        if (equal < 0) {
                            m_Tokens.Enqueue(new OptionToken(OptionTokenKind.LongOption, argument.Substring(2)));
                            m_ArgumentPosition++;
                            return;
                        }
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.LongOption, argument.Substring(2, equal - 2)));
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value, argument.Substring(equal + 1)));
                        m_ArgumentPosition++;
                        return;
                    }

                    // The string -- occurs alone, don't parse any further options. They're all arguments
                    m_ArgumentsOnly = true;
                    return;
                }

                // This is a short option, starts with -
                if (argument.Length > 1) {
                    int equal = argument.IndexOf('=', 1);
                    if (equal >= 0) {
                        if (equal != 2) throw new ArgumentException("Command line token " + argument + "invalid, key value pairs must be isolated");
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption, argument[1].ToString()));
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value, argument.Substring(3)));
                        m_ArgumentPosition++;
                        return;
                    }

                    if (argument.Length > 2) {
                        // There are multiple short options. We've just parsed the very first.
                        m_ArgumentCharPosition = 2;
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption, argument[1].ToString()));
                        return;
                    }

                    m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption, argument[1].ToString()));
                    m_ArgumentPosition++;
                    return;
                }

                // A - alone, is just an argument (e.g. by convention, stdin)
                m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Argument, argument));
                m_ArgumentPosition++;
                return;
            }

            // It's not an option, so this is just an argument
            m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Argument, argument));
            m_ArgumentPosition++;
        }
    }
}
