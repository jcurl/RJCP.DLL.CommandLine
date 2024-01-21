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
}
