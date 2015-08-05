namespace RJCP.Core.CommandLine
{
    /// <summary>
    /// Interface IOptionParser for different flavours of command line parsing.
    /// </summary>
    internal interface IOptionParser
    {
        /// <summary>
        /// Gets the next token, where the specified token type is expected.
        /// </summary>
        /// <param name="expectValue"><c>true</c> if the next token type should be a value for an option.</param>
        /// <returns>The token describing the next token.</returns>
        OptionToken GetToken(bool expectValue);

        /// <summary>
        /// Gets a value indicating whether long options are case insensitive (lower case).
        /// </summary>
        /// <value><c>true</c> if long options are case insensitive; otherwise, <c>false</c>.</value>
        bool LongOptionCaseInsensitive { get; }

        /// <summary>
        /// Gets the list separator character when assigning to lists.
        /// </summary>
        /// <value>The list separator character.</value>
        char ListSeparator { get; }

        /// <summary>
        /// The prefix for short options.
        /// </summary>
        /// <value>The short option prefix.</value>
        string ShortOptionPrefix { get; }

        /// <summary>
        /// The prefix for long options.
        /// </summary>
        /// <value>The long option prefix.</value>
        string LongOptionPrefix { get; }
    }
}
