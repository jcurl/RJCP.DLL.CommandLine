// TODO:
// * Support setting properties that are lists. Arguments are either separated with colon ':' (Windows)
//   or comma ',' (Unix).
// * Implement Windows flavour

namespace RJCP.Core.CommandLine
{
    using System;
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
            BuildOptionList();
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
            BuildOptionList();
        }

        private class OptionData
        {
            public OptionAttribute OptionAttribute { get; set; }
            public FieldInfo Field { get; set; }
            public PropertyInfo Property { get; set; }
            public bool Set { get; set; }

            public bool ExpectsValue
            {
                get
                {
                    if (Field == null) return false;
                    if (Field.FieldType == typeof(bool)) return false;
                    return true;
                }
            }
        }

        private Dictionary<string, OptionData> m_LongOptionList = new Dictionary<string, OptionData>();
        private Dictionary<char, OptionData> m_ShortOptionList = new Dictionary<char, OptionData>();
        private List<OptionData> m_OptionList = new List<OptionData>();

        private void BuildOptionList()
        {
            foreach (FieldInfo field in m_Options.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)) {
                OptionAttribute attribute = GetAttribute<OptionAttribute>(field);
                if (attribute != null) {
                    CheckShortOption(attribute.ShortOption);
                    CheckLongOption(attribute.LongOption);

                    OptionData optionData = new OptionData();
                    optionData.Field = field;
                    AddOption(optionData, attribute);
                }
            }

            foreach (PropertyInfo property in m_Options.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)) {
                OptionAttribute attribute = GetAttribute<OptionAttribute>(property);
                if (attribute != null) {
                    CheckShortOption(attribute.ShortOption);
                    CheckLongOption(attribute.LongOption);

                    OptionData optionData = new OptionData();
                    optionData.Property = property;
                    AddOption(optionData, attribute);
                }
            }

        }

        private void AddOption(OptionData optionData, OptionAttribute attribute)
        {
            optionData.OptionAttribute = attribute;
            m_OptionList.Add(optionData);

            m_ShortOptionList.Add(attribute.ShortOption, optionData);
            if (attribute.LongOption != null) {
                m_LongOptionList.Add(attribute.LongOption, optionData);
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
                SetOption(optionData, argumentToken.Value);
            } else {
                // This is a boolean type. We can only set it to true.
                SetBoolean(optionData, true);
            }

            optionData.Set = true;
        }

        private void ParseLongOption(IOptionParser parser, string value)
        {
            OptionData optionData;
            if (!m_LongOptionList.TryGetValue(value, out optionData)) throw new OptionUnknownException(value);

            if (optionData.ExpectsValue) {
                OptionToken argumentToken = parser.GetToken(true);
                if (argumentToken == null) throw new OptionMissingArgumentException(value);
                SetOption(optionData, argumentToken.Value);
            } else {
                // This is a boolean type. We can only set it to true.
                SetBoolean(optionData, true);
            }

            optionData.Set = true;
        }

        private void SetOption(OptionData optionData, string value)
        {
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
