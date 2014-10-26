namespace RJCP.Core.CommandLine
{
    /// <summary>
    /// The type of token found when parsing the command line.
    /// </summary>
    internal enum OptionTokenKind
    {
        /// <summary>
        /// A generic argument.
        /// </summary>
        Argument,

        /// <summary>
        /// A short option.
        /// </summary>
        ShortOption,

        /// <summary>
        /// A long option.
        /// </summary>
        LongOption,

        /// <summary>
        /// A value assigned to an option.
        /// </summary>
        Value,
    }

    /// <summary>
    /// A small class for maintaining parsing the command line
    /// </summary>
    internal class OptionToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionToken"/> class.
        /// </summary>
        /// <param name="token">The token identified on the command line.</param>
        /// <param name="value">The value associated with the token.</param>
        public OptionToken(OptionTokenKind token, string value)
        {
            Token = token;
            Value = value;
        }

        /// <summary>
        /// Gets the token identified on the command line.
        /// </summary>
        /// <value>The token identified on the command line..</value>
        public OptionTokenKind Token { get; set; }

        /// <summary>
        /// Gets the value associated with the token.
        /// </summary>
        /// <value>The value associated with the token.</value>
        public string Value { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Value;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <param name="parser">The parser.</param>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        /// <remarks>
        /// This method differs that the format of the string can change depending on
        /// the parser being used.
        /// </remarks>
        public virtual string ToString(IOptionParser parser)
        {
            switch (Token) {
            case OptionTokenKind.ShortOption:
                return parser.ShortOptionPrefix + Value;
            case OptionTokenKind.LongOption:
                return parser.LongOptionPrefix + Value;
            case OptionTokenKind.Argument:
                return Value;
            case OptionTokenKind.Value:
                return Value;
            default:
                return Value;
            }
        }
    }
}
