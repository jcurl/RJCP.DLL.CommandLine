// TODO:
// * Implement Windows flavour.
// * Implement Help Usage.
// * How are command line options quoted?
// * string types may be optional, where if no argument provided, it's set to string.empty.
// * Allow long options without short options
//

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

            m_ShortOptionList.Add(attribute.ShortOption, optionData);
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
            IOptionParser parser = new UnixOptionParser(arguments);

            BuildOptionList(parser.LongOptionCaseInsenstive);

            do {
                token = parser.GetToken(false);
                if (token != null) {
                    switch (token.Token) {
                    case OptionTokenKind.ShortOption:
                        if (m_Arguments.Count > 0) {
                            if (lastOptionToken != null) {
                                throw new OptionException("Unexpected option '" + token.Value +
                                                          "', perhaps too many arguments after '" +
                                                          lastOptionToken.Value + "'");
                            }
                            throw new OptionException("Unexpected option '" + token.Value + "'");
                        }
                        ParseShortOption(parser, token.Value);
                        lastOptionToken = token;
                        break;
                    case OptionTokenKind.LongOption:
                        if (m_Arguments.Count > 0) {
                            if (lastOptionToken != null) {
                                throw new OptionException("Unexpected option \"" + token.Value +
                                                          "\", perhaps too many arguments after '" +
                                                          lastOptionToken.Value + "'");
                            }
                            throw new OptionException("Unexpected option \"" + token.Value + "\"");
                        }
                        ParseLongOption(parser, token.Value);
                        lastOptionToken = token;
                        break;
                    case OptionTokenKind.Option:
                        if (m_Arguments.Count > 0) {
                            if (lastOptionToken != null) {
                                throw new OptionException("Unexpected option '" + token.Value +
                                                          "', perhaps too many arguments after '" +
                                                          lastOptionToken.Value + "'");
                            }
                            throw new OptionException("Unexpected option '" + token.Value + "'");
                        }
                        ParseOption(parser, token.Value);
                        lastOptionToken = token;
                        break;
                    case OptionTokenKind.Argument:
                        ParseArgument(parser, token.Value);
                        break;
                    case OptionTokenKind.Value:
                        if (lastToken != null) {
                            throw new OptionException("Unexpected value for option " + lastToken.Value +
                                                        " (argument " + token.Value + ")");
                        }
                        throw new OptionException("Unexpected value " + token.Value);
                    }
                    lastToken = token;
                }
            } while (token != null);

            // Check that all mandatory options were provided
            StringBuilder sb = new StringBuilder();
            foreach (OptionData option in m_OptionList) {
                if (option.OptionAttribute.Required && !option.Set) {
                    sb.Append(option.OptionAttribute.ShortOption);
                }
            }
            if (sb.Length > 0) throw new OptionMissingException(sb.ToString());
        }

        private void ParseShortOption(IOptionParser parser, string value)
        {
            OptionData optionData;
            if (!m_ShortOptionList.TryGetValue(value[0], out optionData)) throw new OptionUnknownException(value);

            if (optionData.ExpectsValue) {
                OptionToken argumentToken = parser.GetToken(true);
                if (argumentToken == null) throw new OptionMissingArgumentException(value);
                SetOption(parser, optionData, argumentToken.Value);
            } else {
                // This is a boolean type. We can only set it to true.
                SetBoolean(optionData, true);
            }

            optionData.Set = true;
        }

        private void ParseLongOption(IOptionParser parser, string value)
        {
            OptionData optionData;
            string option = parser.LongOptionCaseInsenstive ? value.ToLowerInvariant() : value;
            if (!m_LongOptionList.TryGetValue(option, out optionData)) throw new OptionUnknownException(value);

            if (optionData.ExpectsValue) {
                OptionToken argumentToken = parser.GetToken(true);
                if (argumentToken == null) throw new OptionMissingArgumentException(value);
                SetOption(parser, optionData, argumentToken.Value);
            } else {
                // This is a boolean type. We can only set it to true.
                SetBoolean(optionData, true);
            }

            optionData.Set = true;
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
                return;
            }

            if (optionData.Property != null) {
                optionData.Property.SetValue(m_Options, ChangeType(value, optionData.Property.PropertyType), null);
                return;
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

        private void ParseOption(IOptionParser parser, string value)
        {
            if (value.Length == 1) {
                ParseShortOption(parser, value);
            } else {
                ParseLongOption(parser, value);
            }
        }

        private void ParseArgument(IOptionParser parser, string value)
        {
            m_Arguments.Add(value);
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
