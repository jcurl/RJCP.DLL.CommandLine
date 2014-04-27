// $URL$
// $Id$
namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using RJCP.Core.Datastructures;
    using System.Diagnostics;

    #region Defining arguments, with custom handlers
    /// <summary>
    /// Specifies if an argument is required or not
    /// </summary>
    public enum OptRequired
    {
        /// <summary>
        /// Argument is optional
        /// </summary>
        Optional,

        /// <summary>
        /// Argument is required
        /// </summary>
        Required
    };

    /// <summary>
    /// Specifies if the arguments parameter is required or not
    /// </summary>
    public enum OptParamRequired
    {
        /// <summary>
        /// No parameters are expected
        /// </summary>
        None,

        /// <summary>
        /// A parameter may be present
        /// </summary>
        Optional,

        /// <summary>
        /// A parameter is required
        /// </summary>
        Required
    }

    /// <summary>
    /// Specifies the type of the arguments parameter if it's provided
    /// </summary>
    public enum OptParamType
    {
        /// <summary>
        /// An argument is not required
        /// </summary>
        None,

        /// <summary>
        /// A generic string
        /// </summary>
        String,

        /// <summary>
        /// Custom check, event handler is called
        /// </summary>
        Custom
    };

    /// <summary>
    /// Specifies an argument that doesn't require custom handling
    /// </summary>
    public class Option
    {
        #region Protected Variables
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private char m_ShortOption;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string m_LongOption;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private OptRequired m_OptRequired;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private OptParamRequired m_ParamRequired;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private OptParamType m_ParamType;
        #endregion

        /// <summary>
        /// Basic argument that can be optionally provided
        /// </summary>
        /// <param name="shortOption">The short option on the command line</param>
        /// <param name="longOption">The long option on the command line</param>
        public Option(char shortOption, string longOption)
            : this(shortOption, longOption, OptRequired.Optional) { }

        /// <summary>
        /// Basic argument that can be required or not
        /// </summary>
        /// <param name="shortOption">The short option on the command line</param>
        /// <param name="longOption">The long option on the command line</param>
        /// <param name="required">If the argument is required or not</param>
        public Option(char shortOption, string longOption, OptRequired required)
            : this(shortOption, longOption, required, OptParamRequired.None, OptParamType.None) { }

        /// <summary>
        /// Argument that is either a string or an integer
        /// </summary>
        /// <param name="shortOption">The short option on the command line</param>
        /// <param name="longOption">The long option on the command line</param>
        /// <param name="required">If the argument is required or not</param>
        /// <param name="param">Indicates if a parameter to the argument is required or not</param>
        /// <param name="paramType">Type of the argument</param>
        public Option(char shortOption, string longOption, OptRequired required, OptParamRequired param, OptParamType paramType)
        {
            if (string.IsNullOrEmpty(longOption)) throw new ArgumentException("Must specify a valid argument", "longOption");
            m_ShortOption = shortOption;
            m_LongOption = longOption;
            m_OptRequired = required;
            m_ParamRequired = param;
            if (param != OptParamRequired.None) {
                if (paramType == OptParamType.None)
                    throw new ArgumentException("param is ArgParameter." + param.ToString() + ", argType may not be ArgType.None", "argType");
                if (paramType == OptParamType.Custom)
                    throw new ArgumentException("param ArgType.Custom is not supported", "argType");
                m_ParamType = paramType;
            } else {
                if (paramType != OptParamType.None)
                    throw new ArgumentException("param is ArgParameter.None, argType must be ArgType.None", "argType");
                m_ParamType = OptParamType.None;
            }
        }

        /// <summary>
        /// The character used for the short option. May be optional
        /// </summary>
        public char ShortOption
        {
            get { return m_ShortOption; }
            protected set { m_ShortOption = value; }
        }

        /// <summary>
        /// The string used for the long option. Must always be provided
        /// </summary>
        public string LongOption
        {
            get { return m_LongOption; }
            protected set { m_LongOption = value; }
        }

        /// <summary>
        /// If the argument is required to be provided or not
        /// </summary>
        public OptRequired OptionRequired 
        { 
            get { return m_OptRequired; }
            protected set { m_OptRequired = value; }
        }

        /// <summary>
        /// If a parameter to the argument must be provided or not
        /// </summary>
        public OptParamRequired ParamRequired 
        { 
            get { return m_ParamRequired; }
            protected set { m_ParamRequired = value; }
        }

        /// <summary>
        /// How to interpret the argument if it's provided
        /// </summary>
        public OptParamType ParamType
        {
            get { return m_ParamType; }
            protected set { m_ParamType = value; }
        }

        /// <summary>
        /// Get a textual representation of this option
        /// </summary>
        /// <remarks>
        /// This method is provided so that you can show the options usage to the user
        /// based on how it's configured. It also serves a second purpose for identifying
        /// options easily while debugging with the Visual Studio IDE.
        /// </remarks>
        /// <returns>The option and its configuration</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (m_OptRequired == OptRequired.Optional) {
                sb.Append('[');
            }
            sb.Append("--");
            sb.Append(m_LongOption);

            switch (m_ParamRequired) {
            case OptParamRequired.None:
                break;
            case OptParamRequired.Optional:
                sb.Append("[=parameter]");
                break;
            case OptParamRequired.Required:
                sb.Append("=parameter");
                break;
            }

            if (m_OptRequired == OptRequired.Optional) {
                sb.Append(']');
            }
            return sb.ToString();
        }
    }

    /// <summary>
    /// Information about the argument being parsed in the event handler ArgValidateEvents
    /// </summary>
    public class OptParamValidatorArgs<T> : EventArgs where T : class,new()
    {
        private Option<T> m_Option;
        private string m_Param;
        private T m_OptionData;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="option">The option being parsed</param>
        /// <param name="param">The argument provided on the command line</param>
        /// <param name="options">An options object</param>
        public OptParamValidatorArgs(Option<T> option, string param, T options)
        {
            m_Option = option;
            m_Param = param;
            m_OptionData = options;
        }

        /// <summary>
        /// The option being parsed
        /// </summary>
        public Option<T> Option { get { return m_Option; } }

        /// <summary>
        /// The argument provided on the command line
        /// </summary>
        public string Parameter { get { return m_Param; } }

        /// <summary>
        /// Data associated with the object
        /// </summary>
        public T OptionData { get { return m_OptionData; } }
    }

    /// <summary>
    /// Delegate type used for custom parsing of an option
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void OptParamValidateEvent<T>(object sender, OptParamValidatorArgs<T> args) where T : class,new();

    /// <summary>
    /// Specifies an argument that supports custom handling
    /// </summary>
    public class Option<T> : Option where T : class,new()
    {
        private OptParamValidateEvent<T> m_Validator;

        /// <summary>
        /// Basic argument that can be optionally provided
        /// </summary>
        /// <param name="shortOption">The short option on the command line</param>
        /// <param name="longOption">The long option on the command line</param>
        public Option(char shortOption, string longOption)
            : base(shortOption, longOption)
        {
            m_Validator = null;
        }

        /// <summary>
        /// Basic argument that can be required or not
        /// </summary>
        /// <param name="shortOption">The short option on the command line</param>
        /// <param name="longOption">The long option on the command line</param>
        /// <param name="required">If the argument is required or not</param>
        public Option(char shortOption, string longOption, OptRequired required)
            : base(shortOption, longOption, required)
        {
            m_Validator = null;
        }

        /// <summary>
        /// Argument that is either a string or an integer
        /// </summary>
        /// <param name="shortOption">The short option on the command line</param>
        /// <param name="longOption">The long option on the command line</param>
        /// <param name="required">If the argument is required or not</param>
        /// <param name="param">If the parameter to the argument is required or not</param>
        /// <param name="argType">Type of the argument</param>
        public Option(char shortOption, string longOption, OptRequired required, OptParamRequired param, OptParamType argType)
            : base(shortOption, longOption, required, param, argType)
        {
            m_Validator = null;
        }

        /// <summary>
        /// Argument that uses a custom handler
        /// </summary>
        /// <param name="shortOption">The short option on the command line</param>
        /// <param name="longOption">The long option on the command line</param>
        /// <param name="required">If the argument is required or not</param>
        /// <param name="param">If the parameter to the argument is required or not</param>
        /// <param name="validator">The callback to check the argument</param>
        public Option(char shortOption, string longOption, OptRequired required, OptParamRequired param, OptParamValidateEvent<T> validator)
            : base(shortOption, longOption, required)
        {
            base.ParamRequired = param;
            if (param != OptParamRequired.None) {
                if (validator == null)
                    throw new ArgumentNullException("validator", "validator may not be null for ArgParameter." + param.ToString());
                base.ParamType = OptParamType.Custom;
                m_Validator = validator;
            } else {
                if (validator != null)
                    throw new ArgumentException("validator must be null for ArgParameter.None", "validator");
                base.ParamType = OptParamType.None;
                m_Validator = null;
            }
        }

        /// <summary>
        /// Custom event handler for interpreting an argument parameter
        /// </summary>
        public OptParamValidateEvent<T> Validator { get { return m_Validator; } }
    }
    #endregion

    #region CommandLine Option specific Exceptions
    /// <summary>
    /// Base class for argument exceptions. This shouldn't be used directly
    /// </summary>
    [Serializable]
    public class BaseArgumentException : System.Exception
    {
        private string m_Option;

        /// <summary>
        /// Default constructor for the exception
        /// </summary>
        protected BaseArgumentException() : base() { }

        /// <summary>
        /// Exception constructor providing the name of the option
        /// </summary>
        /// <param name="option">Name of the option</param>
        protected BaseArgumentException(string option) : base("Option Exception") 
        {
            m_Option = option;
        }

        /// <summary>
        /// Exception constructor providing the name of the option and an inner exception
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="inner">Inner exception</param>
        protected BaseArgumentException(string option, System.Exception inner) : base("Option Exception", inner) 
        {
            m_Option = option;
        }

        /// <summary>
        /// Exception constructor providing the name of the option an an error message
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        protected BaseArgumentException(string option, string message) : base(message)
        {
            m_Option = option;
        }

        /// <summary>
        /// Exception constructor
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        /// <param name="inner">Inner exception</param>
        protected BaseArgumentException(string option, string message, System.Exception inner) : base(message, inner)
        {
            m_Option = option;
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <remarks>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client. 
        /// </remarks>
        /// <param name="info">Serialization</param>
        /// <param name="context">Context</param>
        protected BaseArgumentException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base (info, context)
        {
            m_Option = info.GetString("option");
        }
        
        /// <summary>
        /// Serialization
        /// </summary>
        /// <param name="info">Serialization Info</param>
        /// <param name="context">Serialization Context</param>
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("option", m_Option);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Get the exception as a string
        /// </summary>
        /// <returns>
        /// String representation of the exception
        /// </returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(m_Option)) {
                return "Option: " + m_Option + "; " + base.ToString();
            } else {
                return base.ToString();
            }
        }

        /// <summary>
        /// The option affected. May be null
        /// </summary>
        public string Option { get { return m_Option; } }
    }

    /// <summary>
    /// A required argument isn't provided on the command line
    /// </summary>
    [Serializable]
    public class MissingOptionException : BaseArgumentException
    {
        /// <summary>
        /// Argument is missing
        /// </summary>
        public MissingOptionException() : base() { }

        /// <summary>
        /// Argument is missing
        /// </summary>
        /// <param name="option">Name of the option</param>
        public MissingOptionException(string option) : base(option, "Missing option") { }

        /// <summary>
        /// Argument is missing
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        public MissingOptionException(string option, string message) : base(option, message) { }

        /// <summary>
        /// Argument is missing
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="inner">Inner exception</param>
        public MissingOptionException(string option, System.Exception inner) : base(option, "Missing option", inner) { }

        /// <summary>
        /// Argument is missing
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        /// <param name="inner">Inner exception</param>
        public MissingOptionException(string option, string message, System.Exception inner) : base(option, message, inner) { }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <remarks>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client. 
        /// </remarks>
        /// <param name="info">Serialization</param>
        /// <param name="context">Context</param>
        protected MissingOptionException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// An argument is provided but the required parameter is not provided
    /// </summary>
    [Serializable]
    public class MissingOptionParameterException : BaseArgumentException
    {
        /// <summary>
        /// Argument is missing parameter
        /// </summary>
        public MissingOptionParameterException() : base() { }

        /// <summary>
        /// Argument is missing parameter
        /// </summary>
        /// <param name="option">Name of the option</param>
        public MissingOptionParameterException(string option) : base(option, "Missing option parameter") { }

        /// <summary>
        /// Argument is missing parameter
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        public MissingOptionParameterException(string option, string message) : base(option, message) { }

        /// <summary>
        /// Argument is missing parameter
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="inner">Inner exception</param>
        public MissingOptionParameterException(string option, System.Exception inner) : base(option, "Missing option parameter", inner) { }

        /// <summary>
        /// Argument is missing parameter
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        /// <param name="inner">Inner exception</param>
        public MissingOptionParameterException(string option, string message, System.Exception inner) : base(option, message, inner) { }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <remarks>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client. 
        /// </remarks>
        /// <param name="info">Serialization</param>
        /// <param name="context">Context</param>
        protected MissingOptionParameterException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Argument parameter is in the wrong format
    /// </summary>
    [Serializable]
    public class OptionParameterException : BaseArgumentException
    {
        /// <summary>
        /// Argument parameter is invalid
        /// </summary>
        public OptionParameterException() : base() { }

        /// <summary>
        /// Argument parameter is invalid
        /// </summary>
        /// <param name="option">Name of the option</param>
        public OptionParameterException(string option) : base(option, "Option parameter format invalid") { }
        
        /// <summary>
        /// Argument parameter is invalid
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        public OptionParameterException(string option, string message) : base(option, message) { }
        
        /// <summary>
        /// Argument parameter is invalid
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="inner">Inner exception</param>
        public OptionParameterException(string option, System.Exception inner) : base(option, "Option parameter format invalid", inner) { }
        
        /// <summary>
        /// Argument parameter is invalid
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        /// <param name="inner">Inner exception</param>
        public OptionParameterException(string option, string message, System.Exception inner) : base(option, message, inner) { }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <remarks>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client. 
        /// </remarks>
        /// <param name="info">Serialization</param>
        /// <param name="context">Context</param>
        protected OptionParameterException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// A parameter was given to an option where no parameter is expected
    /// </summary>
    [Serializable]
    public class ParameterNotOptionalException : BaseArgumentException
    {
        /// <summary>
        /// Argument parameter not expected, but one provided
        /// </summary>
        public ParameterNotOptionalException() : base() { }

        /// <summary>
        /// Argument parameter not expected, but one provided
        /// </summary>
        /// <param name="option">Name of the option</param>
        public ParameterNotOptionalException(string option) : base(option, "Option expects no parameters") { }

        /// <summary>
        /// Argument parameter not expected, but one provided
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        public ParameterNotOptionalException(string option, string message) : base(option, message) { }

        /// <summary>
        /// Argument parameter not expected, but one provided
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="inner">Inner exception</param>
        public ParameterNotOptionalException(string option, System.Exception inner) : base(option, "Option expects no parameters", inner) { }

        /// <summary>
        /// Argument parameter not expected, but one provided
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        /// <param name="inner">Inner exception</param>
        public ParameterNotOptionalException(string option, string message, System.Exception inner) : base(option, message, inner) { }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <remarks>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client. 
        /// </remarks>
        /// <param name="info">Serialization</param>
        /// <param name="context">Context</param>
        protected ParameterNotOptionalException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// An unknown option was encountered
    /// </summary>
    [Serializable]
    public class OptionNotDefinedException : BaseArgumentException
    {
        /// <summary>
        /// Unknown option
        /// </summary>
        public OptionNotDefinedException() : base() { }

        /// <summary>
        /// Unknown option
        /// </summary>
        /// <param name="option">Name of the option</param>
        public OptionNotDefinedException(string option) : base(option, "Unknown option") { }

        /// <summary>
        /// Unknown option
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        public OptionNotDefinedException(string option, string message) : base(option, message) { }

        /// <summary>
        /// Unknown option
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="inner">Inner exception</param>
        public OptionNotDefinedException(string option, System.Exception inner) : base(option, "Unknown option", inner) { }

        /// <summary>
        /// Unknown option
        /// </summary>
        /// <param name="option">Name of the option</param>
        /// <param name="message">Message describing the exception</param>
        /// <param name="inner">Inner exception</param>
        public OptionNotDefinedException(string option, string message, System.Exception inner) : base(option, message, inner) { }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <remarks>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client. 
        /// </remarks>
        /// <param name="info">Serialization</param>
        /// <param name="context">Context</param>
        protected OptionNotDefinedException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }

    /// <summary>
    /// Option configuration is ambiguous
    /// </summary>
    /// <remarks>
    /// This exception can be raised if the configuration for the option is ambiguous. For example,
    /// the user provides a list of short options, where two or more are expecting arguments in the
    /// form "-abc". The user should instead provide "-a -b -c" for it to be non-ambiguous
    /// </remarks>
    [Serializable]
    public class OptionParameterAmbiguousException: BaseArgumentException
    {
        private string m_Option2;

        /// <summary>
        /// Option configuration is ambiguous
        /// </summary>
        public OptionParameterAmbiguousException() : base() { }

        /// <summary>
        /// Option configuration is ambiguous
        /// </summary>
        /// <param name="option">Name of the first option</param>
        /// <param name="option2">Name of the second option</param>
        public OptionParameterAmbiguousException(string option, string option2)
            : base(option + ", " + option2, "Ambiguous Options provided") 
        { 
            m_Option2 = option2;
        }

        /// <summary>
        /// Option configuration is ambiguous
        /// </summary>
        /// <param name="option">Name of the first option</param>
        /// <param name="option2">Name of the second option</param>
        /// <param name="message">Message describing the exception</param>
        public OptionParameterAmbiguousException(string option, string option2, string message)
            : base(option + ", " + option2, message) 
        {
            m_Option2 = option2;
        }

        /// <summary>
        /// Option configuration is ambiguous
        /// </summary>
        /// <param name="option">Name of the first option</param>
        /// <param name="option2">Name of the second option</param>
        /// <param name="inner">Inner exception</param>
        public OptionParameterAmbiguousException(string option, string option2, System.Exception inner)
            : base(option + ", " + option2, "Unknown option", inner)
        {
            m_Option2 = option2;
        }

        /// <summary>
        /// Option configuration is ambiguous
        /// </summary>
        /// <param name="option">Name of the first option</param>
        /// <param name="option2">Name of the second option</param>
        /// <param name="message">Message describing the exception</param>
        /// <param name="inner">Inner exception</param>
        public OptionParameterAmbiguousException(string option, string option2, string message, System.Exception inner)
            : base(option + ", " + option2, message, inner)
        {
            m_Option2 = option2;
        }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <remarks>
        /// A constructor is needed for serialization when an
        /// exception propagates from a remoting server to the client. 
        /// </remarks>
        /// <param name="info">Serialization</param>
        /// <param name="context">Context</param>
        protected OptionParameterAmbiguousException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) 
        {
            m_Option2 = info.GetString("option2");
        }

        /// <summary>
        /// Serialization
        /// </summary>
        /// <param name="info">Serialization Info</param>
        /// <param name="context">Serialization Context</param>
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            info.AddValue("option2", m_Option2);
 	        base.GetObjectData(info, context);
        }

        /// <summary>
        /// Get the exception as a string
        /// </summary>
        /// <returns>
        /// String representation of the exception
        /// </returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(m_Option2)) {
                return "Option: " + m_Option2 + "; " + base.ToString();
            } else {
                return base.ToString();
            }
        }
    }
    #endregion

    /// <summary>
    /// Class for interpreting command line arguments where custom handling is not required
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Internal mapping from long options to an Argument
        /// </summary>
        protected Dictionary<string, Option> m_LongOptions = new Dictionary<string, Option>();

        /// <summary>
        /// Internal mapping from short options to an Argument
        /// </summary>
        protected Dictionary<char, Option> m_ShortOptions = new Dictionary<char, Option>();

        /// <summary>
        /// Parsed options
        /// </summary>
        protected Dictionary<string, string> m_RawOptions = new Dictionary<string, string>();

        /// <summary>
        /// List of all arguments that could not be parsed
        /// </summary>
        protected List<string> m_Unparsed = new List<string>();

        /// <summary>
        /// Constructor, parsing options from the command line
        /// </summary>
        /// <param name="options">List of arguments defining what's supported</param>
        /// <param name="args">Options provided by the user on the command line</param>
        public Options(Option[] options, string[] args)
        {
            if (options == null || options.Length == 0)
                throw new ArgumentException("Must provide an array of arguments", "arguments");

            // Generate an internal mapping for parsing our arguments
            foreach (Option arg in options) {
                m_LongOptions.Add(arg.LongOption, arg);
                if (arg.ShortOption != (char)0) m_ShortOptions.Add(arg.ShortOption, arg);
            } 

            ParseOptions(args);

            CheckMissingOptions();
        }

        private void ParseOptions(string[] args) 
        {
            bool parsing = true;
            Option lastOpt = null;

            foreach (string arg in args) {
                if (parsing) {
                    if (lastOpt == null || lastOpt.ParamRequired == OptParamRequired.None || lastOpt.ParamRequired == OptParamRequired.Optional) {
                        // We expect a new option here, or a parameter for the last option
                        if (!string.IsNullOrEmpty(arg)) {
                            if (arg.Length > 1 && arg[0] == '-') {
                                if (arg[1] == '-') {
                                    if (arg.Length > 2) {
                                        // Long option
                                        LongOptionArgument o = new LongOptionArgument(arg);
                                        Option opt;
                                        if (m_LongOptions.TryGetValue(o.LongOption, out opt)) {
                                            lastOpt = opt;
                                            if (lastOpt.ParamRequired == OptParamRequired.None) {
                                                if (o.Parameter != null) {
                                                    // User provided a parameter for an option that doesn't accept a parameter
                                                    throw new ParameterNotOptionalException(o.LongOption);
                                                }
                                                AddParsed(lastOpt, null);
                                                lastOpt = null;
                                            } else {
                                                if (o.Parameter != null) {
                                                    AddParsed(opt, o.Parameter);
                                                    lastOpt = null;
                                                }
                                                // else on parsing the next argument the parameter will be provided
                                            }
                                        } else {
                                            // User provided an unsupported long option
                                            throw new OptionNotDefinedException(o.LongOption);
                                        }
                                    } else {
                                        // End of option parsing. All following options must be unparsed
                                        if (lastOpt != null) {
                                            if (lastOpt.ParamRequired == OptParamRequired.Optional) {
                                                // Parameter not provided, we add it
                                                AddParsed(lastOpt, null);
                                            } else if (lastOpt.ParamRequired == OptParamRequired.Required) {
                                                // Parameter not provided, but we expect one.
                                                throw new MissingOptionParameterException(lastOpt.LongOption);
                                            }
                                        }
                                        parsing = false;
                                        lastOpt = null;
                                    }
                                } else {
                                    // Collection of short options
                                    lastOpt = null;
                                    for (int i = 1; i < arg.Length; i++) {
                                        char opt = arg[i];
                                        Option so;
                                        if (m_ShortOptions.TryGetValue(opt, out so)) {
                                            if (so.ParamRequired == OptParamRequired.Required || so.ParamRequired == OptParamRequired.Optional) {
                                                if (lastOpt != null) {
                                                    // We only keep lastOpt if it has a required/optional parameter
                                                    throw new OptionParameterAmbiguousException(so.LongOption, lastOpt.LongOption);
                                                }
                                                lastOpt = so;
                                            } else {
                                                AddParsed(so, null);
                                            }
                                        } else {
                                            throw new OptionNotDefinedException(opt.ToString());
                                        }
                                    }
                                }
                            } else {
                                // Parameter for last option; or unparsed option
                                if (lastOpt != null && lastOpt.ParamRequired == OptParamRequired.Optional) {
                                    AddParsed(lastOpt, arg);
                                } else {
                                    AddUnparsed(arg);
                                }
                                lastOpt = null;
                            }
                        } else {
                            // Empty argument. Parameter for last option; or unparsed option
                            if (lastOpt != null && lastOpt.ParamRequired == OptParamRequired.Optional) {
                                AddParsed(lastOpt, arg);
                            } else {
                                AddUnparsed(arg);
                            }
                            lastOpt = null;
                        }
                    } else {
                        // We expect a parameter here
                        AddParsed(lastOpt, arg);
                        lastOpt = null;
                    }
                } else {
                    // We've finishing parsing the command line options
                    AddUnparsed(arg);
                }
            }

            // No more arguments to parse. Check if we're expecting a parameter for any outstanding
            // options.
            if (lastOpt != null) {
                if (lastOpt.ParamRequired == OptParamRequired.Optional) {
                    // Parameter not provided, we add it
                    AddParsed(lastOpt, null);
                } else if (lastOpt.ParamRequired == OptParamRequired.Required) {
                    // Parameter not provided, but we expect one.
                    throw new MissingOptionParameterException(lastOpt.LongOption);
                }
            }
        }

        private void CheckMissingOptions()
        {
            foreach (Option arg in m_LongOptions.Values) {
                if (arg.OptionRequired == OptRequired.Required) {
                    if (!m_RawOptions.ContainsKey(arg.LongOption)) {
                        throw new MissingOptionException(arg.LongOption);
                    }
                }
            }
        }

        /// <summary>
        /// A simple class to parse a single argument from a long option
        /// </summary>
        private class LongOptionArgument
        {
            private string m_LongOption;
            private string m_Parameter;

            /// <summary>
            /// Construct the constituents of the long option
            /// </summary>
            /// <param name="argument">The argument to parse as a long option</param>
            public LongOptionArgument(string argument)
            {
                if (string.IsNullOrEmpty(argument) || argument.Length < 3 ||
                    argument[0] != '-' || argument[1] != '-') {
                    // Not a valid option string
                    m_LongOption = null;
                    m_Parameter = null;
                    return;
                }

                int param = argument.IndexOf('=');
                if (param != -1) {
                    // String contains a parameter
                    if (param < argument.Length - 1) {
                        m_LongOption = argument.Substring(2, param - 2).TrimEnd();
                        m_Parameter = argument.Substring(param + 1).Trim();
                    } else {
                        m_LongOption = argument.Substring(2, param - 2).TrimEnd();
                        m_Parameter = "";
                    }
                } else {
                    m_LongOption = argument.Substring(2).TrimEnd();
                    m_Parameter = null;
                }
            }

            /// <summary>
            /// The long option. Null indicates an invalid argument was provided in the constructor
            /// </summary>
            public string LongOption { get { return m_LongOption; } }

            /// <summary>
            /// The parameter. Null indicates no parameter was provided. An 
            /// empty string indicates a parameter was provided
            /// </summary>
            public string Parameter { get { return m_Parameter; } }
        }

        /// <summary>
        /// Used to add a new option to our list of options parsed
        /// </summary>
        /// <param name="option">The long argument that was parsed</param>
        /// <param name="parameter">The (optional) parameter for the argument. Null indicates no parameter was provided</param>
        protected virtual void AddParsed(Option option, string parameter)
        {
            if (m_RawOptions.ContainsKey(option.LongOption)) {
                m_RawOptions[option.LongOption] = parameter;
            } else {
                m_RawOptions.Add(option.LongOption, parameter);
            }
        }

        /// <summary>
        /// Add an argument to the list of unparsed arguments
        /// </summary>
        /// <param name="argument">The argument string that wasn't parsed</param>
        protected void AddUnparsed(string argument)
        {
            m_Unparsed.Add(argument);
        }

        /// <summary>
        /// Dictionary of (unparsed) options provided on the command line
        /// </summary>
        public IDictionary<string, string> RawOptions
        {
            get { return new ReadOnlyDictionary<string, string>(m_RawOptions); }
        }

        /// <summary>
        /// Arguments that were not interpreted as a command (e.g. a file list, etc.)
        /// </summary>
        public string[] RemainingArguments { get { return m_Unparsed.ToArray(); } }
    }

    /// <summary>
    /// Class for interpreting command line arguments where custom handling may be required
    /// </summary>
    /// <typeparam name="T">A type that will allow the results of custom handling to be stored</typeparam>
    public class Options<T> : Options where T : class,new()
    {
        /// <summary>
        /// List of options based on custom events
        /// </summary>
        private T m_UserOptions = null;

        /// <summary>
        /// Parse command line arguments given a definition of the arguments we should understand
        /// </summary>
        /// <param name="options">List of arguments defined for the program</param>
        /// <param name="args">Arguments provided on the command line</param>
        public Options(Option[] options, string[] args)
            :base(options, args) { }

        /// <summary>
        /// Used to add a new option to our list of options parsed
        /// </summary>
        /// <param name="option">The long argument that was parsed</param>
        /// <param name="parameter">The (optional) parameter for the argument. Null indicates no parameter was provided</param>
        protected override void AddParsed(Option option, string parameter)
        {
            if (option.ParamType == OptParamType.Custom) {
                OnArgValidate((Option<T>)option, parameter);
            }

            base.AddParsed(option, parameter);
        }

        /// <summary>
        /// Call the user handler for parsing an argument
        /// </summary>
        /// <param name="option">The option</param>
        /// <param name="parameter">The argument provided on the command line</param>
        protected void OnArgValidate(Option<T> option, string parameter)
        {
            if (m_UserOptions == null) {
                m_UserOptions = new T();
            }

            // The base class only knows about "Argument", but it is actually of type "Argument<T>"
            option.Validator(this, new OptParamValidatorArgs<T>(option, parameter, m_UserOptions));
        }

        /// <summary>
        /// User parsed options, filled out by the custom event handlers
        /// </summary>
        public T UserOptions
        {
            get { return m_UserOptions; }
        }
    }
}
