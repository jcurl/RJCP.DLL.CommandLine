namespace RJCP.Core.CommandLine
{
    using System;

    /// <summary>
    /// Class OptionDefaultAttribute. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// Provides a default value for the option, if it is given without an argument.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class OptionDefaultAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionDefaultAttribute"/> class.
        /// </summary>
        /// <param name="defaultValue">The default value as a string. This value is converted
        /// to the correct type.</param>
        public OptionDefaultAttribute(string defaultValue)
        {
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Gets the default value.
        /// </summary>
        /// <value>The default value.</value>
        public string DefaultValue { get; private set; }
    }
}
