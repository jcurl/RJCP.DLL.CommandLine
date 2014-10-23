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
        /// An option (unknown if short or long)
        /// </summary>
        Option,

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
    }
}
