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
        bool LongOptionCaseInsenstive { get; }

        /// <summary>
        /// Gets the list separator character when assigning to lists.
        /// </summary>
        /// <value>The list separator character.</value>
        char ListSeparator { get; }
    }
}
