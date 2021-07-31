namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Resources;

    /// <summary>
    /// The style of command line options to use.
    /// </summary>
    public enum OptionsStyle
    {
        /// <summary>
        /// Windows style.
        /// </summary>
        Windows,

        /// <summary>
        /// Unix style.
        /// </summary>
        Unix
    }

    /// <summary>
    /// Object to parse the command line options and set the fields within the class provided in the constructor.
    /// </summary>
    /// <remarks>
    /// To parse command line options, create a new class of your choosing that should contain the results of
    /// command line parsing. You should define a set of properties and fields. It's those properties and fields
    /// which will be set by this class when parsing the command line.
    /// <para>The properties and fields may have any kind of visibility you need, e.g. <c>public</c>, <c>private</c>,
    /// <c>internal</c>, <c>internal protected</c> or <c>protected</c> when using C#. Even the <c>private</c> fields
    /// can be set through reflection.</para>
    /// <para>Instantiate your class and then pass the reference to the method <see cref="Parse(object, string[])"/> with
    /// the command line parameters provided by the main entry point.</para>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// using System;
    /// namespace CommandLineTest {
    ///   class Program {
    ///     static void Main(string[] args) {
    ///       CmdLineOptions myOptions = new CmdLineOptions();
    ///       Options.Parse(myOptions, args, OptionsStyle.Unix);
    ///     }
    ///   }
    /// }
    /// ]]></code>
    /// </example>
    /// <para>The class <c>CmdLineOptions</c> needs to be defined by your project. Properties that should
    /// be set are decorated with the <see cref="OptionAttribute"/> attribute.</para>
    /// <example>
    /// <code language="csharp"><![CDATA[
    /// private class CmdLineOptions {
    ///   [Option('a', "along", false)]
    ///   public bool OptionA;
    ///
    ///   [Option('b', "blong", false)]
    ///   public bool OptionB;
    ///
    ///   [Option('c', "clong", false)]
    ///   public string OptionC;
    /// }
    /// ]]></code>
    /// </example>
    /// <para>So if your program is given the option "-a", the property <c>CmdLineOptions.OptionA</c> is automatically
    /// set to <see langword="true"/>. The variable <c>OptionC</c> is set to the argument that follows, e.g. <c>-c mystring</c>.</para>
    /// <para>Properties with the type <c>boolean</c> may not have arguments and are always set to <see langword="true"/> when
    /// provided. All types except <c>boolean</c> types expect an argument.</para>
    /// <note type="note">This application can only parse the command line that comes from Windows. In particular,
    /// Windows removes any quoting which may result in unexpected behavior. Such an example is <c>--foo="a,b",c</c>
    /// which results in a single argument <c>--foo=a,b,c</c> which is indistinguishable from the command line
    /// <c>--foo="a,b,c"</c> that also results in <c>--foo=a,b,c</c>. In this case, the user should use the single
    /// tick for quoting.</note>
    /// </remarks>
    public class Options
    {
        /// <summary>
        /// Parses the command line arguments writing to options.
        /// </summary>
        /// <param name="options">The options object to write to.</param>
        /// <param name="arguments">The argument list to parse.</param>
        /// <returns>An instance of this object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> may not be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// Not a property or field - An <see cref="OptionAttribute"/> was assigned to something that wasn't a property or a field.
        /// </exception>
        /// <exception cref="OptionException">
        /// <see cref="OptionArgumentsAttribute"/> assigned to multiple fields / properties. You may only assign optional arguments
        /// to a single field / property.
        /// <para>- or -</para>
        /// Unexpected option provided.
        /// <para>- or -</para>
        /// Unexpected value for option provided.
        /// <para>- or -</para>
        /// Error parsing option.
        /// <para>- or -</para>
        /// Wrong format given to option.
        /// <para>- or -</para>
        /// Invalid data after quoted list.
        /// <para>- or -</para>
        /// Missing quote in list.
        /// </exception>
        /// <exception cref="OptionDuplicateException">
        /// The same short option or long option was specified with the <see cref="OptionAttribute"/> multiple times.
        /// </exception>
        /// <exception cref="OptionMissingException">
        /// Mandatory options were not provided on the command line.
        /// </exception>
        /// <exception cref="OptionUnknownException">
        /// Unknown option provided on the command line.
        /// </exception>
        /// <exception cref="OptionAssignedException">
        /// Option has already been provided where only one instance allowed.
        /// </exception>
        /// <remarks>
        /// The default style used for command line options is dependent on the operating system that this program is
        /// running on. For Windows NT, the <see cref="OptionsStyle.Windows"/> is used. For Unix like operating systems
        /// (such as Linux and MacOS), the <see cref="OptionsStyle.Unix"/> is used. Use the constructor
        /// <see cref="Options.Parse(object, string[], OptionsStyle)"/> to override this.
        /// </remarks>
        public static Options Parse(object options, string[] arguments)
        {
            Options cmdLine = new Options(options);
            if (Platform.IsWinNT()) {
                cmdLine.OptionsStyle = OptionsStyle.Windows;
            } else {
                cmdLine.OptionsStyle = OptionsStyle.Unix;
            }
            cmdLine.ParseCommandLine(arguments);
            return cmdLine;
        }

        /// <summary>
        /// Parses the specified options.
        /// </summary>
        /// <param name="options">The options object to write to.</param>
        /// <param name="arguments">The argument list to parse.</param>
        /// <param name="style">The option style to use.</param>
        /// <returns>An instance of this object.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="options"/> may not be <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        /// Not a property or field - An <see cref="OptionAttribute"/> was assigned to something that wasn't a property or a field.
        /// <para>- or -</para>
        /// An unknown value for <paramref name="style"/> was given.
        /// </exception>
        /// <exception cref="OptionException">
        /// <see cref="OptionArgumentsAttribute"/> assigned to multiple fields / properties. You may only assign optional arguments
        /// to a single field / property.
        /// <para>- or -</para>
        /// Unexpected option provided.
        /// <para>- or -</para>
        /// Unexpected value for option provided.
        /// <para>- or -</para>
        /// Error parsing option.
        /// <para>- or -</para>
        /// Wrong format given to option.
        /// <para>- or -</para>
        /// Invalid data after quoted list.
        /// <para>- or -</para>
        /// Missing quote in list.
        /// </exception>
        /// <exception cref="OptionDuplicateException">
        /// The same short option or long option was specified with the <see cref="OptionAttribute"/> multiple times.
        /// </exception>
        /// <exception cref="OptionMissingException">
        /// Mandatory options were not provided on the command line.
        /// </exception>
        /// <exception cref="OptionUnknownException">
        /// Unknown option provided on the command line.
        /// </exception>
        /// <exception cref="OptionAssignedException">
        /// Option has already been provided where only one instance allowed.
        /// </exception>
        public static Options Parse(object options, string[] arguments, OptionsStyle style)
        {
            Options cmdLine = new Options(options) {
                OptionsStyle = style
            };
            cmdLine.ParseCommandLine(arguments);
            return cmdLine;
        }

        private readonly object m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class using the Windows style.
        /// </summary>
        /// <param name="options">The object to receive the options for.</param>
        private Options(object options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            m_Options = options;

            Arguments = new ReadOnlyCollection<string>(m_Arguments);
        }

        #region Inspecting and building command line options
        private readonly Dictionary<string, OptionMember> m_LongOptionList = new Dictionary<string, OptionMember>();
        private readonly Dictionary<char, OptionMember> m_ShortOptionList = new Dictionary<char, OptionMember>();
        private readonly List<OptionMember> m_OptionList = new List<OptionMember>();
        private readonly IList<string> m_Arguments = new List<string>();
        private OptionField m_ArgumentsField;

        private void BuildOptionList(bool longOptionCaseInsensitive)
        {
            Type optionsClassType = m_Options.GetType();

            while (optionsClassType != null) {
                FieldInfo[] fields = optionsClassType.GetFields(
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (FieldInfo field in fields) {
                    ParseMember(field, longOptionCaseInsensitive);
                }

                PropertyInfo[] properties = optionsClassType.GetProperties(
                    BindingFlags.Instance | BindingFlags.Static |
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                foreach (PropertyInfo property in properties) {
                    ParseMember(property, longOptionCaseInsensitive);
                }

                optionsClassType = optionsClassType.BaseType;
            }
        }

        private void ParseMember(MemberInfo memberInfo, bool longOptionCaseInsensitive)
        {
            OptionAttribute attribute = GetAttribute<OptionAttribute>(memberInfo);
            if (attribute != null) {
                CheckShortOption(attribute.ShortOption);
                CheckLongOption(attribute.LongOption);

                OptionMember optionMember = new OptionMember(m_Options, attribute, memberInfo);
                m_OptionList.Add(optionMember);

                if (attribute.ShortOption != (char)0) {
                    m_ShortOptionList.Add(attribute.ShortOption, optionMember);
                }
                if (attribute.LongOption != null) {
                    string longOption = longOptionCaseInsensitive
                        ? attribute.LongOption.ToLowerInvariant()
                        : attribute.LongOption;
                    m_LongOptionList.Add(longOption, optionMember);
                }
                return;
            }

            OptionArgumentsAttribute argAttribute = GetAttribute<OptionArgumentsAttribute>(memberInfo);
            if (argAttribute != null) {
                if (m_ArgumentsField != null)
                    throw new OptionException(CmdLineStrings.OptionArguments_AssignedMultipleTimes);

                m_ArgumentsField = new OptionField(memberInfo);
                if (!m_ArgumentsField.IsList)
                    throw new OptionException(CmdLineStrings.OptionArguments_RequiresCollection);
                if (m_ArgumentsField.ListType != typeof(string) && m_ArgumentsField.ListType != typeof(object))
                    throw new OptionException(CmdLineStrings.OptionArguments_GenTypeString);
            }
        }

        private void CheckShortOption(char shortOption)
        {
            if (shortOption == (char)0) return;

            if (m_ShortOptionList.ContainsKey(shortOption)) {
                string message = string.Format(CmdLineStrings.OptionMultipleTimesSpecified, shortOption);
                throw new OptionDuplicateException(message);
            }
            if (m_LongOptionList.ContainsKey(shortOption.ToString())) {
                string message = string.Format(CmdLineStrings.OptionMultipleTimesSpecified, shortOption);
                throw new OptionDuplicateException(message);
            }
        }

        private void CheckLongOption(string longOption)
        {
            if (longOption == null) return;

            if (m_LongOptionList.ContainsKey(longOption)) {
                string message = string.Format(CmdLineStrings.OptionMultipleTimesSpecified, longOption);
                throw new OptionDuplicateException(message);
            }
            if (longOption.Length == 1) {
                if (m_ShortOptionList.ContainsKey(longOption[0])) {
                    string message = string.Format(CmdLineStrings.OptionMultipleTimesSpecified, longOption);
                    throw new OptionDuplicateException(message);
                }
            }
        }

        private static T GetAttribute<T>(ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), false).OfType<T>().FirstOrDefault();
        }
        #endregion

        private OptionsStyle m_OptionsStyle;

        /// <summary>
        /// Gets or sets the options style to use when parsing the command line.
        /// </summary>
        /// <value>The options style to use when parsing the command line.</value>
        /// <exception cref="ArgumentException">Unknown Options Style</exception>
        public OptionsStyle OptionsStyle
        {
            get { return m_OptionsStyle; }
            private set
            {
                if (!Enum.IsDefined(typeof(OptionsStyle), value))
                    throw new ArgumentException(CmdLineStrings.OptionsStyleUnknown, nameof(value));
                m_OptionsStyle = value;
            }
        }

        /// <summary>
        /// Parses the command line arguments.
        /// </summary>
        /// <param name="arguments">The command line arguments to parse.</param>
        /// <remarks>
        /// Parses the command line arguments provided, setting the fields in the object provided when
        /// this object was instantiated.
        /// </remarks>
        private void ParseCommandLine(string[] arguments)
        {
            OptionToken token;
            OptionToken lastToken = null;
            OptionToken lastOptionToken = null;
            IOptionParser parser;

            switch (m_OptionsStyle) {
            case OptionsStyle.Unix:
                parser = new UnixOptionParser(arguments);
                break;
            case OptionsStyle.Windows:
                parser = new WindowsOptionEnumerator(arguments);
                break;
            default:
                throw new InvalidOperationException(CmdLineStrings.OptionsStyleUnknown);
            }

            BuildOptionList(parser.LongOptionCaseInsensitive);
            IOptions options = m_Options as IOptions;
            try {
                do {
                    string message;
                    token = parser.GetToken(false);
                    if (token == null) continue;
                    switch (token.Token) {
                    case OptionTokenKind.ShortOption:
                    case OptionTokenKind.LongOption:
                        if (m_Arguments.Count > 0) {
                            if (lastOptionToken != null) {
                                message = string.Format(CmdLineStrings.UnexpectedOptionNonZeroGeneralArgsLastToken,
                                    token.ToString(parser), lastOptionToken.ToString(parser));
                            } else {
                                message = string.Format(CmdLineStrings.UnexpectedOptionNonZeroGeneralArgs, token.ToString(parser));
                            }
                            throw new OptionException(message);
                        }
                        ParseOption(parser, token);
                        lastOptionToken = token;
                        break;
                    case OptionTokenKind.Argument:
                        ParseArgument(token);
                        break;
                    case OptionTokenKind.Value:
                        if (lastToken != null) {
                            message = string.Format(CmdLineStrings.UnexpectedValueForOptionLastToken,
                                lastToken.ToString(parser), token);
                            throw new OptionException(message);
                        }
                        message = string.Format(CmdLineStrings.UnexpectedValueForOption, token);
                        throw new OptionException(message);
                    }
                    lastToken = token;
                } while (token != null);
            } catch (OptionUnknownException e) {
                if (options != null) options.InvalidOption(e.Option);
                throw;
            } catch (OptionMissingArgumentException e) {
                if (options != null) options.InvalidOption(e.Option);
                throw;
            } catch (OptionFormatException e) {
                if (options != null) options.InvalidOption(e.Option);
                throw;
            } catch (OptionException) {
                if (options != null) options.Usage();
                throw;
            }

            // Check that all mandatory options were provided
            StringBuilder sb = new StringBuilder();
            List<string> optionList = new List<string>();
            foreach (OptionMember optionMember in m_OptionList) {
                if (optionMember.Attribute.Required && !optionMember.IsSet) {
                    MissingOption(parser,
                        optionMember.Attribute.ShortOption,
                        optionMember.Attribute.LongOption,
                        sb, optionList);
                }
            }

            if (sb.Length > 0) {
                if (options != null) options.Missing(optionList);
                throw new OptionMissingException(sb.ToString());
            }

            if (options != null) options.Check();
        }

        private void MissingOption(IOptionParser parser, char shortOption, string longOption, StringBuilder message, ICollection<string> missing)
        {
            if (message.Length != 0) message.Append(", ");

            if (shortOption != (char)0) {
                message.Append(parser.ShortOptionPrefix).Append(shortOption);
                if (missing != null) missing.Add(shortOption.ToString());
            } else if (longOption != null) {
                message.Append(parser.LongOptionCaseInsensitive).Append(longOption);
                if (missing != null) missing.Add(longOption);
            }
        }

        private void ParseOption(IOptionParser parser, OptionToken token)
        {
            switch (token.Token) {
            case OptionTokenKind.ShortOption:
                ParseShortOption(parser, token);
                break;
            case OptionTokenKind.LongOption:
                ParseLongOption(parser, token);
                break;
            }
        }

        private void ParseShortOption(IOptionParser parser, OptionToken token)
        {
            if (!m_ShortOptionList.TryGetValue(token.Value[0], out OptionMember optionMember))
                throw new OptionUnknownException(token.ToString(parser));

            ParseOptionParameter(parser, optionMember, token);
        }

        private void ParseLongOption(IOptionParser parser, OptionToken token)
        {
            string option = parser.LongOptionCaseInsensitive ? token.Value.ToLowerInvariant() : token.Value;
            if (!m_LongOptionList.TryGetValue(option, out OptionMember optionMember))
                throw new OptionUnknownException(token.ToString(parser));

            ParseOptionParameter(parser, optionMember, token);
        }

        private void ParseArgument(OptionToken token)
        {
            m_Arguments.Add(token.Value);
            if (m_ArgumentsField != null) m_ArgumentsField.Add(m_Options, token.Value);
        }

        private void ParseOptionParameter(IOptionParser parser, OptionMember optionMember, OptionToken token)
        {
            string argument = null;
            try {
                if (optionMember.ExpectsValue) {
                    OptionToken argumentToken = parser.GetToken(true);
                    if (argumentToken == null) {
                        OptionDefaultAttribute defaultAttribute =
                            GetAttribute<OptionDefaultAttribute>(optionMember.Member);
                        if (defaultAttribute == null)
                            throw new OptionMissingArgumentException(token.ToString(parser));
                        argument = defaultAttribute.DefaultValue;
                    } else {
                        argument = argumentToken.Value;
                    }

                    if (optionMember.IsList) {
                        SplitList(optionMember, parser.ListSeparator, argument);
                    } else {
                        if (optionMember.IsSet) throw new OptionAssignedException(argument);
                        optionMember.SetValue(argument);
                    }
                } else {
                    // This is a boolean type. We can only set it to true.
                    optionMember.SetValue(true);
                }
                optionMember.IsSet = true;
            } catch (OptionException) {
                throw;
            } catch (Exception e) {
                string message;
                if (argument == null) {
                    message = string.Format(CmdLineStrings.ErrorParsingOption, token.ToString(parser));
                    throw new OptionException(message);
                }
                message = string.Format(CmdLineStrings.ErrorParsingOptionFormat, argument, token.ToString(parser));
                throw new OptionFormatException(token.Value, message, e);
            }
        }

        private void SplitList(OptionMember optionMember, char separationChar, string value)
        {
            StringBuilder sb = new StringBuilder();
            char quote = (char)0;
            int schar = 0;
            bool escape = false;
            bool quoted = false;

            for (int i = 0; i < value.Length; i++) {
                char c = value[i];
                if (escape) {
                    escape = false;

                    // We only escape quotes, else every other character is literal. This allows
                    // Windows paths to be given without having to escape each path character.
                    if (c == '\'' || c == '\"') continue;
                    sb.Append('\\');
                    continue;
                }
                if (c == '\\') {
                    // Copy from 'schar' to this character to cut out the
                    // escape character.
                    sb.Append(value.Substring(schar, i - schar));
                    escape = true;
                    schar = i + 1;
                    continue;
                }
                if (c == '\'' || c == '\"') {
                    if (quote == 0) {
                        // Begin a quote
                        schar = i + 1;
                        quote = value[i];
                        quoted = true;
                        continue;
                    }
                    if (quote == value[i]) {
                        // End of the quote
                        sb.Append(value.Substring(schar, i - schar));
                        schar = i + 1;
                        quote = (char)0;
                        continue;
                    }
                }
                if (c == separationChar && quote == (char)0) {
                    sb.Append(value.Substring(schar, i - schar));
                    optionMember.AddValue(sb.ToString());
                    sb.Clear();
                    schar = i + 1;
                    quoted = false;
                    continue;
                }

                if (quote == (char)0 && quoted) {
                    string message = string.Format(CmdLineStrings.ListInvalidData, separationChar);
                    throw new OptionException(message);
                }
            }

            if (escape) sb.Append('\\');

            if (quote != (char)0) throw new OptionException(CmdLineStrings.ListMissingQuote);

            // Add the trailing option
            sb.Append(value.Substring(schar));
            optionMember.AddValue(sb.ToString());
        }

        /// <summary>
        /// Gets the arguments that were not parsed as options.
        /// </summary>
        /// <value>The arguments not parsed as options.</value>
        public IList<string> Arguments { get; private set; }
    }
}
