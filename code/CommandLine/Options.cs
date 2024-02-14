namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Resources;
    using RJCP.Core.Environment;

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
        /// Gets the default options style if not overridden in the class constructors.
        /// </summary>
        /// <value>The default options style.</value>
        public static OptionsStyle DefaultOptionsStyle
        {
            get
            {
                if (!Platform.IsWinNT() || Platform.IsMSys() || Platform.IsCygwin()) {
                    return OptionsStyle.Unix;
                } else {
                    return OptionsStyle.Windows;
                }
            }
        }

        /// <summary>
        /// Parses the command line arguments writing to options.
        /// </summary>
        /// <param name="options">
        /// The options object to write to. If <see langword="null"/> the returns an "empty" options object. No
        /// arguments are parsed. This is useful to get properties about the commandline.
        /// </param>
        /// <param name="arguments">The argument list to parse.</param>
        /// <returns>An instance of this object.</returns>
        /// <exception cref="ArgumentException">
        /// Not a property or field - An <see cref="OptionAttribute"/> was assigned to something that wasn't a property
        /// or a field.
        /// <para>- or -</para>
        /// <see cref="OptionArgumentsAttribute"/> assigned more than once, to multiple fields / properties
        /// <para>- or -</para>
        /// <see cref="OptionArgumentsAttribute"/> may not be assigned to a <c>readonly</c> field
        /// <para>- or -</para>
        /// <see cref="OptionArgumentsAttribute"/> is not a collection, or is not a collection of strings
        /// <para>- or -</para>
        /// A property has no setter
        /// <para>- or -</para>
        /// A field is readonly
        /// <para>- or -</para>
        /// The short option from <see cref="OptionAttribute"/> is used on more than one field or property
        /// <para>- or -</para>
        /// The long option from <see cref="OptionAttribute"/> is used on more than one field or property
        /// <para>- or -</para>
        /// Types must be simple types (primitive types, bool or string types), which apply also to collections.
        /// </exception>
        /// <exception cref="OptionMissingException">Mandatory options were not provided on the command line.</exception>
        /// <exception cref="OptionUnknownException">Unknown option provided on the command line.</exception>
        /// <exception cref="OptionAssignedException">
        /// Option has already been provided where only one instance allowed.
        /// </exception>
        /// <exception cref="OptionException">
        /// User provided an option after a general argument has been seen on the command line.
        /// <para>- or -</para>
        /// User provided a list but quotes are not formatted correctly, or missing quotes.
        /// <para>- or -</para>
        /// User provided an unexpected value to an option.
        /// </exception>
        /// <remarks>
        /// The default style used for command line options is dependent on the operating system that this program is
        /// running on. For Windows NT, the <see cref="OptionsStyle.Windows"/> is used. For Unix like operating systems
        /// (such as Linux and MacOS), the <see cref="OptionsStyle.Unix"/> is used. Use the constructor
        /// <see cref="Options.Parse(object, string[], OptionsStyle)"/> to override this.
        /// </remarks>
        public static Options Parse(object options, string[] arguments)
        {
            return Parse(options, arguments, DefaultOptionsStyle);
        }

        /// <summary>
        /// Parses the specified options.
        /// </summary>
        /// <param name="options">
        /// The options object to write to. If <see langword="null"/> the returns an "empty" options object. No
        /// arguments are parsed. This is useful to get properties about the commandline.
        /// </param>
        /// <param name="arguments">The argument list to parse.</param>
        /// <param name="style">The option style to use.</param>
        /// <returns>An instance of this object.</returns>
        /// <exception cref="ArgumentException">
        /// Not a property or field - An <see cref="OptionAttribute"/> was assigned to something that wasn't a property or a field.
        /// <para>- or -</para>
        /// <see cref="OptionArgumentsAttribute"/> assigned more than once, to multiple fields / properties
        /// <para>- or -</para>
        /// <see cref="OptionArgumentsAttribute"/> may not be assigned to a <c>readonly</c> field
        /// <para>- or -</para>
        /// <see cref="OptionArgumentsAttribute"/> is not a collection, or is not a collection of strings
        /// <para>- or -</para>
        /// A property has no setter
        /// <para>- or -</para>
        /// A field is readonly
        /// <para>- or -</para>
        /// The short option from <see cref="OptionAttribute"/> is used on more than one field or property
        /// <para>- or -</para>
        /// The long option from <see cref="OptionAttribute"/> is used on more than one field or property
        /// <para>- or -</para>
        /// Types must be simple types (primitive types, bool or string types), which apply also to collections.
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
        /// <exception cref="OptionException">
        /// User provided an option after a general argument has been seen on the command line.
        /// <para>- or -</para>
        /// User provided a list but quotes are not formatted correctly, or missing quotes.
        /// <para>- or -</para>
        /// User provided an unexpected value to an option.
        /// </exception>
        public static Options Parse(object options, string[] arguments, OptionsStyle style)
        {
            Options cmdLine = new(options, style);
            cmdLine.ParseCommandLine(arguments);
            return cmdLine;
        }

        private readonly object m_Options;
        private readonly IOptionParser m_Parser;

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class using the Windows style.
        /// </summary>
        /// <param name="options">
        /// The options object to write to. If <see langword="null"/> the returns an "empty" options object. No
        /// arguments are parsed. This is useful to get properties about the commandline.
        /// </param>
        /// <param name="style">The option style to use.</param>
        private Options(object options, OptionsStyle style)
        {
            switch (style) {
            case OptionsStyle.Unix:
                m_Parser = new UnixOptionParser();
                break;
            case OptionsStyle.Windows:
                m_Parser = new WindowsOptionEnumerator();
                break;
            default:
                throw new InvalidOperationException(CmdLineStrings.OptionsStyleUnknown);
            }
            m_Options = options;

            Arguments = new ReadOnlyCollection<string>(m_Arguments);
        }

        #region Inspecting and building command line options
        private readonly Dictionary<string, OptionMember> m_LongOptionList = new();
        private readonly Dictionary<char, OptionMember> m_ShortOptionList = new();
        private readonly List<OptionMember> m_OptionList = new();
        private readonly IList<string> m_Arguments = new List<string>();
        private OptionField m_ArgumentsField;

        private void BuildOptionList(bool longOptionCaseInsensitive)
        {
            Type optionsClassType = m_Options.GetType();

            while (optionsClassType is not null) {
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
            if (attribute is not null) {
                CheckShortOption(attribute.ShortOption);
                CheckLongOption(attribute.LongOption);

                OptionMember optionMember = new(m_Options, attribute, memberInfo);
                m_OptionList.Add(optionMember);

                if (attribute.ShortOption != (char)0) {
                    m_ShortOptionList.Add(attribute.ShortOption, optionMember);
                }
                if (attribute.LongOption is not null) {
                    string longOption = longOptionCaseInsensitive
                        ? attribute.LongOption.ToLowerInvariant()
                        : attribute.LongOption;
                    m_LongOptionList.Add(longOption, optionMember);
                }
                return;
            }

            OptionArgumentsAttribute argAttribute = GetAttribute<OptionArgumentsAttribute>(memberInfo);
            if (argAttribute is not null) {
                if (m_ArgumentsField is not null)
                    throw new ArgumentException(CmdLineStrings.ArgException_AssignedMultipleTimes);
                m_ArgumentsField = new OptionField(memberInfo, true, typeof(string));
            }
        }

        private void CheckShortOption(char shortOption)
        {
            if (shortOption == (char)0) return;

            if (m_ShortOptionList.ContainsKey(shortOption)) {
                string message = string.Format(CmdLineStrings.ArgException_OptionMultipleTimesSpecified, shortOption);
                throw new ArgumentException(message);
            }
            if (m_LongOptionList.ContainsKey(shortOption.ToString())) {
                string message = string.Format(CmdLineStrings.ArgException_OptionMultipleTimesSpecified, shortOption);
                throw new ArgumentException(message);
            }
        }

        private void CheckLongOption(string longOption)
        {
            if (longOption is null) return;

            if (m_LongOptionList.ContainsKey(longOption)) {
                string message = string.Format(CmdLineStrings.ArgException_OptionMultipleTimesSpecified, longOption);
                throw new ArgumentException(message);
            }
            if (longOption.Length == 1) {
                if (m_ShortOptionList.ContainsKey(longOption[0])) {
                    string message = string.Format(CmdLineStrings.ArgException_OptionMultipleTimesSpecified, longOption);
                    throw new ArgumentException(message);
                }
            }
        }

        private static T GetAttribute<T>(ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), false).OfType<T>().FirstOrDefault();
        }
        #endregion

        /// <summary>
        /// The prefix for short options.
        /// </summary>
        /// <value>The short option prefix.</value>
        public string ShortOptionPrefix { get { return m_Parser.ShortOptionPrefix; } }

        /// <summary>
        /// The prefix for long options.
        /// </summary>
        /// <value>The long option prefix.</value>
        public string LongOptionPrefix { get { return m_Parser.LongOptionPrefix; } }

        /// <summary>
        /// Gets the assignment symbol.
        /// </summary>
        /// <value>The assignment symbol.</value>
        public string AssignmentSymbol { get { return m_Parser.AssignmentSymbol; } }

        /// <summary>
        /// Gets or sets the options style to use when parsing the command line.
        /// </summary>
        /// <value>The options style to use when parsing the command line.</value>
        public OptionsStyle OptionsStyle { get { return m_Parser.Style; } }

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
            // Nothing to parse.
            if (m_Options is null) return;

            OptionToken token;
            OptionToken lastToken = null;
            OptionToken lastOptionToken = null;

            m_Parser.AddArguments(arguments);
            BuildOptionList(m_Parser.LongOptionCaseInsensitive);
            IOptions options = m_Options as IOptions;
            try {
                do {
                    string message;
                    token = m_Parser.GetToken(false);
                    if (token is null) continue;
                    switch (token.Token) {
                    case OptionTokenKind.ShortOption:
                    case OptionTokenKind.LongOption:
                        if (m_Arguments.Count > 0) {
                            if (lastOptionToken is not null) {
                                message = string.Format(CmdLineStrings.UnexpectedOptionNonZeroGeneralArgsLastToken,
                                    token.ToString(m_Parser), lastOptionToken.ToString(m_Parser));
                            } else {
                                message = string.Format(CmdLineStrings.UnexpectedOptionNonZeroGeneralArgs, token.ToString(m_Parser));
                            }
                            throw new OptionException(message);
                        }
                        ParseOption(m_Parser, token);
                        lastOptionToken = token;
                        break;
                    case OptionTokenKind.Argument:
                        ParseArgument(token);
                        break;
                    case OptionTokenKind.Value:
                        if (lastToken is not null) {
                            message = string.Format(CmdLineStrings.UnexpectedValueForOptionLastToken,
                                lastToken.ToString(m_Parser), token);
                            throw new OptionException(message);
                        }
                        message = string.Format(CmdLineStrings.UnexpectedValueForOption, token);
                        throw new OptionException(message);
                    }
                    lastToken = token;
                } while (token is not null);
            } catch (OptionUnknownException e) {
                if (options is not null) options.InvalidOption(e.Option);
                throw;
            } catch (OptionMissingArgumentException e) {
                if (options is not null) options.InvalidOption(e.Option);
                throw;
            } catch (OptionFormatException e) {
                if (options is not null) options.InvalidOption(e.Option);
                throw;
            } catch (OptionException) {
                if (options is not null) options.Usage();
                throw;
            }

            // Check that all mandatory options were provided
            StringBuilder sb = new();
            List<string> optionList = new();
            foreach (OptionMember optionMember in m_OptionList) {
                if (optionMember.Attribute.Required && !optionMember.IsSet) {
                    MissingOption(m_Parser,
                        optionMember.Attribute.ShortOption,
                        optionMember.Attribute.LongOption,
                        sb, optionList);
                }
            }

            if (sb.Length > 0) {
                if (options is not null) options.Missing(optionList);
                throw new OptionMissingException(sb.ToString());
            }

            if (options is not null) options.Check();
        }

        private static void MissingOption(IOptionParser parser, char shortOption, string longOption, StringBuilder message, ICollection<string> missing)
        {
            if (message.Length != 0) message.Append(", ");

            if (shortOption != (char)0) {
                message.Append(parser.ShortOptionPrefix).Append(shortOption);
                if (missing is not null) missing.Add(shortOption.ToString());
            } else if (longOption is not null) {
                message.Append(parser.LongOptionCaseInsensitive).Append(longOption);
                if (missing is not null) missing.Add(longOption);
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
            if (m_ArgumentsField is not null) m_ArgumentsField.Add(m_Options, token.Value);
        }

        private static void ParseOptionParameter(IOptionParser parser, OptionMember optionMember, OptionToken token)
        {
            string argument = null;
            try {
                if (optionMember.ExpectsValue) {
                    OptionToken argumentToken = parser.GetToken(true);
                    if (argumentToken is null) {
                        OptionDefaultAttribute defaultAttribute =
                            GetAttribute<OptionDefaultAttribute>(optionMember.Member)
                                ?? throw new OptionMissingArgumentException(token.ToString(parser));
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
            } catch (Exception ex) {
                string message;
                if (argument is null) {
                    message = string.Format(CmdLineStrings.ErrorParsingOption, token.ToString(parser));
                    throw new OptionException(message);
                }

                if (ex is TargetInvocationException reflectEx) {
                    // Exception is caused by setting the property through reflection.
                    if (reflectEx.InnerException is OptionException)
                        throw reflectEx.InnerException;
                    ex = reflectEx;
                }
                message = string.Format(CmdLineStrings.ErrorParsingOptionFormat, argument, token.ToString(parser));
                throw new OptionFormatException(token.Value, message, ex);
            }
        }

        private static void SplitList(OptionMember optionMember, char separationChar, string value)
        {
            StringBuilder sb = new();
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
                    if (c is '\'' or '\"') continue;
                    sb.Append('\\');
                    continue;
                }
                if (c == '\\') {
                    // Copy from 'schar' to this character to cut out the
                    // escape character.
#if NETFRAMEWORK
                    sb.Append(value.Substring(schar, i - schar));
#else
                    sb.Append(value.AsSpan(schar, i - schar));
#endif
                    escape = true;
                    schar = i + 1;
                    continue;
                }
                if (c is '\'' or '\"') {
                    if (quote == 0) {
                        // Begin a quote
                        schar = i + 1;
                        quote = value[i];
                        quoted = true;
                        continue;
                    }
                    if (quote == value[i]) {
                        // End of the quote
#if NETFRAMEWORK
                        sb.Append(value.Substring(schar, i - schar));
#else
                        sb.Append(value.AsSpan(schar, i - schar));
#endif
                        schar = i + 1;
                        quote = (char)0;
                        continue;
                    }
                }
                if (c == separationChar && quote == (char)0) {
#if NETFRAMEWORK
                    sb.Append(value.Substring(schar, i - schar));
#else
                    sb.Append(value.AsSpan(schar, i - schar));
#endif
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
#if NETFRAMEWORK
            sb.Append(value.Substring(schar));
#else
            sb.Append(value.AsSpan(schar));
#endif
            optionMember.AddValue(sb.ToString());
        }

        /// <summary>
        /// Gets the arguments that were not parsed as options.
        /// </summary>
        /// <value>The arguments not parsed as options.</value>
        public IList<string> Arguments { get; private set; }
    }
}
