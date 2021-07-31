namespace RJCP.Core.CommandLine
{
    using System.Collections.Generic;

    internal sealed class WindowsOptionEnumerator : IOptionParser
    {
        private string[] m_Arguments;
        private Queue<OptionToken> m_Tokens = new Queue<OptionToken>();

        public WindowsOptionEnumerator(string[] arguments)
        {
            m_Arguments = arguments;
        }

        public bool LongOptionCaseInsensitive { get { return true; } }
        public char ListSeparator { get { return ','; } }
        public string ShortOptionPrefix { get { return "/"; } }
        public string LongOptionPrefix { get { return "/"; } }

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
                    // Didn't find a value/argument
                    return null;
                }
                return m_Tokens.Dequeue();
            }
            return null;
        }

        private int m_ArgumentPosition;
        private void ParseNextArgument(bool expectValue)
        {
            string argument = m_Arguments[m_ArgumentPosition];

            if (argument.Length > 1 && argument[0] == '/') {
                // This is either a long or a short option
                int valueOperator = argument.IndexOf(':', 2);
                if (valueOperator == -1) {
                    if (argument.Length == 2) {
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption, argument.Substring(1)));
                        m_ArgumentPosition++;
                        return;
                    }
                    m_Tokens.Enqueue(new OptionToken(OptionTokenKind.LongOption, argument.Substring(1)));
                    m_ArgumentPosition++;
                    return;
                }

                if (valueOperator == 2) {
                    m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption, argument.Substring(1, 1)));
                    m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value, argument.Substring(3)));
                    m_ArgumentPosition++;
                    return;
                }

                m_Tokens.Enqueue(new OptionToken(OptionTokenKind.LongOption, argument.Substring(1, valueOperator - 1)));
                m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value, argument.Substring(valueOperator + 1)));
                m_ArgumentPosition++;
                return;
            }

            // It's not an option, so this is just an argument
            if (expectValue) {
                m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value, argument));
            } else {
                m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Argument, argument));
            }
            m_ArgumentPosition++;
        }
    }
}
