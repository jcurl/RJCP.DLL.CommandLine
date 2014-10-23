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
    }
}
