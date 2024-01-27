namespace RJCP.Core.CommandLine
{
    using System.Collections.Generic;

    internal sealed class WindowsOptionEnumerator : IOptionParser
    {
        private readonly string[] m_Arguments;

        public WindowsOptionEnumerator(string[] arguments)
        {
            m_Arguments = arguments;
        }

        public bool LongOptionCaseInsensitive { get { return true; } }

        public char ListSeparator { get { return ','; } }

        public string ShortOptionPrefix { get { return "/"; } }

        public string LongOptionPrefix { get { return "/"; } }

        private readonly Queue<OptionToken> m_Tokens = new Queue<OptionToken>();
        private readonly Queue<OptionToken> m_NonOptArgs = new Queue<OptionToken>();

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

        private int m_ArgumentPosition;

        private bool ParseNextArgument(bool expectValue)
        {
            if (m_ArgumentPosition >= m_Arguments.Length) return false;

            string argument = m_Arguments[m_ArgumentPosition];

            if (argument.Length > 1 && argument[0] == '/') {
                // This is either a long or a short option
                int valueOperator = argument.IndexOf(':', 2);
                if (valueOperator == -1) {
                    if (argument.Length == 2) {
                        m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption, argument.Substring(1)));
                        m_ArgumentPosition++;
                        return true;
                    }
                    m_Tokens.Enqueue(new OptionToken(OptionTokenKind.LongOption, argument.Substring(1)));
                    m_ArgumentPosition++;
                    return true;
                }

                if (valueOperator == 2) {
                    m_Tokens.Enqueue(new OptionToken(OptionTokenKind.ShortOption, argument.Substring(1, 1)));
                    m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value, argument.Substring(3)));
                    m_ArgumentPosition++;
                    return true;
                }

                m_Tokens.Enqueue(new OptionToken(OptionTokenKind.LongOption, argument.Substring(1, valueOperator - 1)));
                m_Tokens.Enqueue(new OptionToken(OptionTokenKind.Value, argument.Substring(valueOperator + 1)));
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
