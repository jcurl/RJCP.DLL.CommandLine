namespace RJCP.Core.CommandLine
{
    using System;

    /// <summary>
    /// Class OptionAttribute. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// Decorate fields of your class with this attribute and pass to the
    /// <see cref="Options"/> constructor. The fields where this attribute
    /// is applied will receive the value as on the command line.
    /// </remarks>
    public sealed class OptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class for a short option. Option is optional.
        /// </summary>
        /// <param name="shortOption">The short option character for this field (case sensitive).</param>
        public OptionAttribute(char shortOption)
        {
            CheckShortOption(shortOption);
            ShortOption = shortOption;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class for a short option. The option may be made mandatory.
        /// </summary>
        /// <param name="shortOption">The short option character for this field (case sensitive).</param>
        /// <param name="required">Option will be made mandatory if <see langword="true"/>.</param>
        public OptionAttribute(char shortOption, bool required)
        {
            CheckShortOption(shortOption);
            ShortOption = shortOption;
            Required = required;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class for a short and long option. Option is optional.
        /// </summary>
        /// <param name="shortOption">The short option character for this field (case sensitive).</param>
        /// <param name="longOption">The long option string (case insensitive).</param>
        public OptionAttribute(char shortOption, string longOption)
        {
            CheckShortOption(shortOption);
            CheckLongOption(longOption);
            ShortOption = shortOption;
            LongOption = longOption.Trim();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class for a short and long option. The option may be made mandatory.
        /// </summary>
        /// <param name="shortOption">The short option character for this field (case sensitive).</param>
        /// <param name="longOption">The long option string (case insensitive).</param>
        /// <param name="required">Option will be made mandatory if <see langword="true"/>.</param>
        public OptionAttribute(char shortOption, string longOption, bool required)
        {
            CheckShortOption(shortOption);
            CheckLongOption(longOption);
            ShortOption = shortOption;
            LongOption = longOption;
            Required = required;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class.
        /// </summary>
        /// <param name="longOption">The long option string (case insensitive).</param>
        public OptionAttribute(string longOption)
        {
            CheckLongOption(longOption);
            LongOption = longOption;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAttribute"/> class.
        /// </summary>
        /// <param name="longOption">The long option string (case insensitive).</param>
        /// <param name="required">Option will be made mandatory if <see langword="true"/>.</param>
        public OptionAttribute(string longOption, bool required)
        {
            CheckLongOption(longOption);
            LongOption = longOption;
            Required = required;
        }

        private void CheckLongOption(string longOption)
        {
            if (string.IsNullOrWhiteSpace(longOption))
                throw new ArgumentException("Long Option may not be empty", nameof(longOption));

            for (int i = 0; i < longOption.Length; i++) {
                if (!IsValidLongOptionChar(i, longOption[i])) {
                    string msg = string.Format("Long option has invalid character: {0}", longOption[i]);
                    throw new ArgumentException(msg, nameof(longOption));
                }
            }
        }

        private static readonly char[] ValidLongOptionChars = new char[] { '-', '_' };

        private static bool IsValidLongOptionChar(int pos, char longOptionChar)
        {
            if (char.IsLetter(longOptionChar)) return true;
            if (pos == 0) return false;
            if (char.IsDigit(longOptionChar)) return true;
            if (Array.IndexOf<char>(ValidLongOptionChars, longOptionChar) != -1) return true;
            return false;
        }

        private void CheckShortOption(char shortOption)
        {
            if (!IsValidShortOptionChar(shortOption)) {
                string message = string.Format("Short option '{0}' character not allowed", shortOption);
                throw new ArgumentException(message, nameof(shortOption));
            }
        }

        private static readonly char[] ValidShortOptionChars = new char[] { '-', '_', '!', '+', '?', '#' };

        private static bool IsValidShortOptionChar(char shortOptionChar)
        {
            if (char.IsLetter(shortOptionChar)) return true;
            if (char.IsDigit(shortOptionChar)) return true;
            if (Array.IndexOf(ValidShortOptionChars, shortOptionChar) != -1) return true;
            return false;
        }

        /// <summary>
        /// The short option character (case sensitive)
        /// </summary>
        /// <value>The short option (case sensitive).</value>
        public char ShortOption { get; private set; }

        /// <summary>
        /// Gets the long option string that can be used instead of the short option (case insensitive).
        /// </summary>
        /// <value>The long option (case insensitive).</value>
        public string LongOption { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the option is required (mandatory).
        /// </summary>
        /// <value><see langword="true"/> if required; otherwise, <see langword="false"/>.</value>
        public bool Required { get; private set; }
    }
}
