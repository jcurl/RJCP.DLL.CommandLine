namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

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
    /// <para>Instantiate your class and then pass the reference to the method <see cref="ParseCommandLine"/> with
    /// the command line parameters provided by the main entry point.</para>
    /// <example>
    /// <code>
    /// using System;
    /// namespace CommandLineTest {
    ///   class Program {
    ///     static void Main(string[] args) {
    ///       CmdLineOptions myOptions = new CmdLineOptions();
    ///       Options options = new Options(OptionsStyle.Unix, myOptions);
    ///       options.ParseCommandLine(args);
    ///     }
    ///   }
    /// }
    /// </code>
    /// </example>
    /// <para>The class <c>CmdLineOptions</c> needs to be defined by your project. Properties that should
    /// be set are decorated with the <see cref="OptionAttribute"/> attribute.</para>
    /// <example>
    /// <code>
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
    /// </code>
    /// </example>
    /// <para>So if your program is given the option "-a", the property <c>CmdLineOptions.OptionA</c> is automatically
    /// set to <c>true</c>. The variable <c>OptionC</c> is set to the argument that follows, e.g. <c>-c mystring</c>.</para>
    /// <para>Properties with the type <c>boolean</c> may not have arguments and are always set to <c>true</c> when
    /// provided. All types except <c>boolean</c> types expect an argument.</para>
    /// <note type="note">This application can only parse the command line that comes from Windows. In particular,
    /// Windows removes any quoting which may result in unexpected behaviour. Such an example is <c>--foo="a,b",c</c>
    /// which results in a single argument <c>--foo=a,b,c</c> which is indistiguishable from the command line
    /// <c>--foo="a,b,c"</c> that also results in <c>--foo=a,b,c</c>. In this case, the user should use the single
    /// tick for quoting.</note>
    /// </remarks>
    public class Options
    {
        private object m_Options;

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class using the Windows style.
        /// </summary>
        /// <param name="options">The object to receive the options for.</param>
        public Options(object options)
        {
            if (options == null) throw new ArgumentNullException("options");
            m_Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class using the style specified.
        /// </summary>
        /// <param name="style">The style of command line options given.</param>
        /// <param name="options">The object to receive the options for.</param>
        public Options(OptionsStyle style, object options)
        {
            if (options == null) throw new ArgumentNullException("options");
            OptionsStyle = style;
            m_Options = options;
        }

        private class OptionData
        {
            public OptionAttribute OptionAttribute { get; set; }
            public FieldInfo Field { get; set; }
            public PropertyInfo Property { get; set; }
            public bool Set { get; set; }

            public bool IsList
            {
                get
                {
                    if (Field != null && typeof(IList).IsAssignableFrom(Field.FieldType)) return true;
                    if (Property != null && typeof(IList).IsAssignableFrom(Property.PropertyType)) return true;
                    return false;
                }
            }

            public bool ExpectsValue
            {
                get
                {
                    if (Field != null && Field.FieldType == typeof(bool)) return false;
                    if (Property != null && Property.PropertyType == typeof(bool)) return false;
                    return true;
                }
            }
        }

        private Dictionary<string, OptionData> m_LongOptionList = new Dictionary<string, OptionData>();
        private Dictionary<char, OptionData> m_ShortOptionList = new Dictionary<char, OptionData>();
        private List<OptionData> m_OptionList = new List<OptionData>();

        private void BuildOptionList(bool longOptionCaseInsensitive)
        {
            foreach (FieldInfo field in m_Options.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)) {
                OptionAttribute attribute = GetAttribute<OptionAttribute>(field);
                if (attribute != null) {
                    CheckShortOption(attribute.ShortOption);
                    CheckLongOption(attribute.LongOption);

                    OptionData optionData = new OptionData();
                    optionData.Field = field;
                    AddOption(optionData, attribute, longOptionCaseInsensitive);
                }
            }

            foreach (PropertyInfo property in m_Options.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)) {
                OptionAttribute attribute = GetAttribute<OptionAttribute>(property);
                if (attribute != null) {
                    CheckShortOption(attribute.ShortOption);
                    CheckLongOption(attribute.LongOption);

                    OptionData optionData = new OptionData();
                    optionData.Property = property;
                    AddOption(optionData, attribute, longOptionCaseInsensitive);
                }
            }

        }

        private void AddOption(OptionData optionData, OptionAttribute attribute, bool longOptionCaseInsensitive)
        {
            optionData.OptionAttribute = attribute;
            m_OptionList.Add(optionData);

            if (attribute.ShortOption != (char) 0) {
                m_ShortOptionList.Add(attribute.ShortOption, optionData);
            }
            if (attribute.LongOption != null) {
                string longOption = longOptionCaseInsensitive
                    ? attribute.LongOption.ToLowerInvariant()
                    : attribute.LongOption;
                m_LongOptionList.Add(longOption, optionData);
            }
        }

        [Conditional("DEBUG")]
        private void CheckShortOption(char shortOption)
        {
            if (shortOption == (char) 0) return;

            if (m_ShortOptionList.ContainsKey(shortOption)) {
                throw new OptionDuplicateException("Option '" + shortOption + "' was specified multiple times");
            }
            if (m_LongOptionList.ContainsKey(shortOption.ToString())) {
                throw new OptionDuplicateException("Option \"" + shortOption + "\" was specified multiple times");
            }
        }

        [Conditional("DEBUG")]
        private void CheckLongOption(string longOption)
        {
            if (longOption == null) return;

            if (m_LongOptionList.ContainsKey(longOption)) {
                throw new OptionDuplicateException("Option \"" + longOption + "\" was specified multiple times");
            }
            if (longOption.Length == 1) {
                if (m_ShortOptionList.ContainsKey(longOption[0])) {
                    throw new OptionDuplicateException("Option '" + longOption + "' was specified multiple times");
                }
            }
        }

        private static T GetAttribute<T>(ICustomAttributeProvider provider) where T : Attribute
        {
            return provider.GetCustomAttributes(typeof(T), false).OfType<T>().FirstOrDefault();
        }

        private OptionsStyle m_OptionsStyle;

        /// <summary>
        /// Gets or sets the options style to use when parsing the command line.
        /// </summary>
        /// <value>The options style to use when parsing the command line.</value>
        /// <exception cref="System.ArgumentException">Unknown Options Style;value</exception>
        public OptionsStyle OptionsStyle
        {
            get { return m_OptionsStyle; }
            set
            {
                if (!Enum.IsDefined(OptionsStyle.GetType(), value))
                    throw new ArgumentException("Unknown Options Style", "value");
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
        public void ParseCommandLine(string[] arguments)
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
                throw new ApplicationException("Unknown command style parser");
            }

            BuildOptionList(parser.LongOptionCaseInsenstive);
            IOptions options = m_Options as IOptions;

            try {
                do {
                    token = parser.GetToken(false);
                    if (token != null) {
                        switch (token.Token) {
                            case OptionTokenKind.ShortOption:
                                if (m_Arguments.Count > 0) {
                                    if (lastOptionToken != null) {
                                        throw new OptionException("Unexpected option '" + token.ToString(parser) +
                                                                  "', perhaps too many arguments after '" +
                                                                  lastOptionToken.ToString(parser) + "'");
                                    }
                                    throw new OptionException("Unexpected option '" + token.ToString(parser) + "'");
                                }
                                ParseShortOption(parser, token);
                                lastOptionToken = token;
                                break;
                            case OptionTokenKind.LongOption:
                                if (m_Arguments.Count > 0) {
                                    if (lastOptionToken != null) {
                                        throw new OptionException("Unexpected option '" + token.ToString(parser) +
                                                                  "', perhaps too many arguments after '" +
                                                                  lastOptionToken.ToString(parser) + "'");
                                    }
                                    throw new OptionException("Unexpected option '" + token.ToString(parser) + "'");
                                }
                                ParseLongOption(parser, token);
                                lastOptionToken = token;
                                break;
                            case OptionTokenKind.Argument:
                                ParseArgument(parser, token);
                                break;
                            case OptionTokenKind.Value:
                                if (lastToken != null) {
                                    throw new OptionException("Unexpected value for option " +
                                                              lastToken.ToString(parser) +
                                                              " (argument " + token + ")");
                                }
                                throw new OptionException("Unexpected value " + token);
                        }
                        lastToken = token;
                    }
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
            foreach (OptionData option in m_OptionList) {
                if (option.OptionAttribute.Required && !option.Set) {
                    MissingOption(parser,
                        option.OptionAttribute.ShortOption, option.OptionAttribute.LongOption,
                        sb, optionList);
                }
            }

            if (sb.Length > 0) {
                if (options != null) options.Missing(optionList);
                throw new OptionMissingException(sb.ToString());
            }

            if (options != null) options.Check();
        }

        private void MissingOption(IOptionParser parser, char shortOption, string longOption, StringBuilder message, IList<string> missing)
        {
            if (message.Length != 0) message.Append(", ");

            if (shortOption != (char) 0) {
                message.Append(parser.ShortOptionPrefix).Append(shortOption);
                if (missing != null) missing.Add(shortOption.ToString());
            } else if (longOption != null) {
                message.Append(parser.LongOptionCaseInsenstive).Append(longOption);
                if (missing != null) missing.Add(longOption);
            }
        }

        private void ParseShortOption(IOptionParser parser, OptionToken token)
        {
            OptionData optionData;
            if (!m_ShortOptionList.TryGetValue(token.Value[0], out optionData)) 
                throw new OptionUnknownException(token.ToString(parser));

            ParseOptionParameter(parser, optionData, token);
        }

        private void ParseLongOption(IOptionParser parser, OptionToken token)
        {
            OptionData optionData;
            string option = parser.LongOptionCaseInsenstive ? token.Value.ToLowerInvariant() : token.Value;
            if (!m_LongOptionList.TryGetValue(option, out optionData))
                throw new OptionUnknownException(token.ToString(parser));

            ParseOptionParameter(parser, optionData, token);
        }

        private void ParseArgument(IOptionParser parser, OptionToken token)
        {
            m_Arguments.Add(token.Value);
        }

        private void ParseOptionParameter(IOptionParser parser, OptionData optionData, OptionToken token)
        {
            string argument = null;
            try {
                if (optionData.ExpectsValue) {
                    OptionToken argumentToken = parser.GetToken(true);
                    if (argumentToken == null) {
                        OptionDefaultAttribute defaultAttribute = null;
                        if (optionData.Field != null)
                            defaultAttribute = GetAttribute<OptionDefaultAttribute>(optionData.Field);
                        if (optionData.Property != null)
                            defaultAttribute = GetAttribute<OptionDefaultAttribute>(optionData.Property);
                        if (defaultAttribute == null)
                            throw new OptionMissingArgumentException(token.ToString(parser));
                        argument = defaultAttribute.DefaultValue;
                    } else {
                        argument = argumentToken.Value;
                    }
                    SetOption(parser, optionData, argument);
                } else {
                    // This is a boolean type. We can only set it to true.
                    SetBoolean(optionData, true);
                }

                optionData.Set = true;
            } catch (OptionException) {
                throw;
            } catch (System.Exception e) {
                if (argument == null) {
                    throw new OptionException("Error parsing option '" + token.ToString(parser) + "'");
                }
                throw new OptionFormatException(token.Value, "Wrong format '" + argument + "' given to option " + token.ToString(parser), e);
            }
        }

        private void SetOption(IOptionParser parser, OptionData optionData, string value)
        {
            if (optionData.IsList) {
                IList list = null;
                if (optionData.Field != null) list = (IList)optionData.Field.GetValue(m_Options);
                if (optionData.Property != null) list = (IList)optionData.Property.GetValue(m_Options, null);
                SplitList(list, parser.ListSeparator, value);
                return;
            }

            if (optionData.Set) throw new OptionAssignedException(value);

            if (optionData.Field != null) {
                optionData.Field.SetValue(m_Options, ChangeType(value, optionData.Field.FieldType));
            } else if (optionData.Property != null) {
                optionData.Property.SetValue(m_Options, ChangeType(value, optionData.Property.PropertyType), null);
            }
        }

        private void SetBoolean(OptionData optionData, bool value)
        {
            if (optionData.Field != null) {
                optionData.Field.SetValue(m_Options, value);
                return;
            }
            if (optionData.Property != null) {
                optionData.Property.SetValue(m_Options, value, null);
                return;
            }
        }

        private void SplitList(IList list, char separationChar, string value)
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
                    list.Add(sb.ToString());
                    sb.Clear();
                    schar = i + 1;
                    quoted = false;
                    continue;
                }

                if (quote == (char)0 && quoted) 
                    throw new OptionException("Invalid data after quoted list, expect '" + separationChar + "' only");
            }

            if (escape) throw new OptionException("Invalid list, unfinished escape sequence");

            if (quote != (char)0) throw new OptionException("Missing quote in list");

            // Add the trailing option
            sb.Append(value.Substring(schar));
            list.Add(sb.ToString());
        }

        private static object ChangeType(string value, Type type)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            return converter.ConvertFromInvariantString(value);
        }

        private List<string> m_Arguments = new List<string>();
        private ReadOnlyCollection<string> m_ArgumentsReadOnly;

        /// <summary>
        /// Gets the arguments that were not parsed as options.
        /// </summary>
        /// <value>The arguments not parsed as options.</value>
        public IList<string> Arguments
        {
            get
            {
                if (m_ArgumentsReadOnly == null) m_ArgumentsReadOnly = new ReadOnlyCollection<string>(m_Arguments);
                return m_ArgumentsReadOnly;
            }
        }
    }
}
