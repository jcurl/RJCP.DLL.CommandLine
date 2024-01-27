namespace RJCP.Core.CommandLine
{
    /// <summary>
    /// Interface IOptionParser for different flavors of command line parsing.
    /// </summary>
    internal interface IOptionParser
    {
        /// <summary>
        /// Adds the arguments that are on the command line.
        /// </summary>
        /// <param name="arguments">The arguments.</param>
        void AddArguments(string[] arguments);

        /// <summary>
        /// Gets the next token, where the specified token type is expected.
        /// </summary>
        /// <param name="expectValue"><see langword="true"/> if the next token type should be a value for an option.</param>
        /// <returns>The token describing the next token.</returns>
        OptionToken GetToken(bool expectValue);

        /// <summary>
        /// Gets the options style this parser implements.
        /// </summary>
        /// <value>The options style this parser implements.</value>
        OptionsStyle Style { get; }

        /// <summary>
        /// Gets a value indicating whether long options are case insensitive (lower case).
        /// </summary>
        /// <value><see langword="true"/> if long options are case insensitive; otherwise, <see langword="false"/>.</value>
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

        /// <summary>
        /// Gets the assignment symbol.
        /// </summary>
        /// <value>The assignment symbol.</value>
        string AssignmentSymbol { get; }
    }
}
