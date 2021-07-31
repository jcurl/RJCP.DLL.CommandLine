namespace RJCP.Core.CommandLine
{
    using System;

    /// <summary>
    /// Indicates a property to contain the list of extra arguments for parsing.
    /// </summary>
    /// <remarks>
    /// Decorate a single property that implements a <see cref="System.Collections.Generic.IList{T}"/> type.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class OptionArgumentsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionArgumentsAttribute"/> class.
        /// </summary>
        public OptionArgumentsAttribute() { }
    }
}
