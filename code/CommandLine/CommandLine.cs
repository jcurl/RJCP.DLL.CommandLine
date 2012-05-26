// $URL$
// $Id$
using System;
using System.Collections.Generic;
using System.Text;
using RJCP.Core.Datastructures;

namespace RJCP.Core.CommandLine
{
    /// <summary>
    /// Specifies if an argument is required or not
    /// </summary>
    public enum ArgRequired
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
    public enum ArgParameter
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
    public enum ArgType
    {
        /// <summary>
        /// An argument is not required
        /// </summary>
        None,

        /// <summary>
        /// An integer type
        /// </summary>
        Int,

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
    public class Argument
    {
        protected char m_ShortOption;
        protected string m_LongOption;
        protected ArgRequired m_ArgRequired;
        protected ArgParameter m_ArgParameter;
        protected ArgType m_ArgType;

        /// <summary>
        /// Basic argument that can be optionally provided
        /// </summary>
        /// <param name="shortOption">The short option on the command line</param>
        /// <param name="longOption">The long option on the command line</param>
        public Argument(char shortOption, string longOption)
            : this(shortOption, longOption, ArgRequired.Optional) { }

        /// <summary>
        /// Basic argument that can be required or not
        /// </summary>
        /// <param name="shortOption">The short option on the command line</param>
        /// <param name="longOption">The long option on the command line</param>
        /// <param name="required">If the argument is required or not</param>
        public Argument(char shortOption, string longOption, ArgRequired required)
            : this(shortOption, longOption, required, ArgParameter.None, ArgType.None) { }

        /// <summary>
        /// Argument that is either a string or an integer
        /// </summary>
        /// <param name="shortOption">The short option on the command line</param>
        /// <param name="longOption">The long option on the command line</param>
        /// <param name="required">If the argument is required or not</param>
        /// <param name="argType">Type of the argument</param>
        public Argument(char shortOption, string longOption, ArgRequired required, ArgParameter param, ArgType argType)
        {
            if (string.IsNullOrEmpty(longOption)) throw new ArgumentException("Must specify a valid argument", "longOption");
            if (argType != ArgType.Int && argType != ArgType.String)
                throw new ArgumentException("Must specify an argument type of Int or String", "argType");
            m_ShortOption = shortOption;
            m_LongOption = longOption;
            m_ArgRequired = required;
            m_ArgParameter = param;
            if (param != ArgParameter.None) {
                if (argType == ArgType.None)
                    throw new ArgumentException("param is ArgParameter." + param.ToString() + ", argType may not be ArgType.None", "argType");
                if (argType == ArgType.Custom)
                    throw new ArgumentException("param ArgType.Custom is not supported", "argType");
                m_ArgType = argType;
            } else {
                if (argType != ArgType.None)
                    throw new ArgumentException("param is ArgParameter.None, argType must be ArgType.None", "argType");
                m_ArgType = ArgType.None;
            }
        }

        /// <summary>
        /// The character used for the short option. May be optional
        /// </summary>
        public char ShortOption { get { return m_ShortOption; } }

        /// <summary>
        /// The string used for the long option. Must always be provided
        /// </summary>
        public string LongOption { get { return m_LongOption; } }

        /// <summary>
        /// If the argument is required to be provided or not
        /// </summary>
        public ArgRequired Required { get { return m_ArgRequired; } }

        /// <summary>
        /// If a parameter to the argument must be provided or not
        /// </summary>
        public ArgParameter Parameter { get { return m_ArgParameter; } }

        /// <summary>
        /// How to interpret the argument if it's provided
        /// </summary>
        public ArgType ArgType { get { return m_ArgType; } }
    }

    /// <summary>
    /// Information about the argument being parsed in the event handler ArgValidateEvents
    /// </summary>
    public class ArgValidatorArgs<T> : EventArgs where T : class,new()
    {
        private string m_LongOption;
        private string m_Param;
        private T m_Options;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="longOption">The long option being parsed</param>
        /// <param name="arg">The argument provided on the command line</param>
        /// <param name="options">An options object</param>
        public ArgValidatorArgs(string longOption, string param, T options)
        {
            m_LongOption = longOption;
            m_Param = param;
            m_Options = options;
        }

        public string LongOption { get { return m_LongOption; } }
        public string Parameter { get { return m_Param; } }
        public T Options { get { return m_Options; } }
    }

    /// <summary>
    /// Delegate type used for custom parsing of an option
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public delegate void ArgValidateEvent<T>(object sender, ArgValidatorArgs<T> args) where T:class,new();

    /// <summary>
    /// Specifies an argument that supports custom handling
    /// </summary>
    public class Argument<T> : Argument where T:class,new()
    {
        protected ArgValidateEvent<T> m_Validator;

        /// <summary>
        /// Basic argument that can be optionally provided
        /// </summary>
        /// <param name="shortOption">The short option on the command line</param>
        /// <param name="longOption">The long option on the command line</param>
        public Argument(char shortOption, string longOption)
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
        public Argument(char shortOption, string longOption, ArgRequired required)
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
        /// <param name="argType">Type of the argument</param>
        public Argument(char shortOption, string longOption, ArgRequired required, ArgParameter param, ArgType argType)
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
        /// <param name="validator">The callback to check the argument</param>
        public Argument(char shortOption, string longOption, ArgRequired required, ArgParameter param, ArgValidateEvent<T> validator)
            : base(shortOption, longOption, required)
        {
            m_ArgParameter = param;
            if (param != ArgParameter.None) {
                if (validator == null)
                    throw new ArgumentNullException("validator", "validator may not be null for ArgParameter." + param.ToString());
                m_ArgType = ArgType.Custom;
                m_Validator = validator;
            } else {
                if (validator != null)
                    throw new ArgumentException("validator must be null for ArgParameter.None", "validator");
                m_ArgType = ArgType.None;
                m_Validator = null;
            }
        }

        /// <summary>
        /// Custom event handler for interpreting an argument parameter
        /// </summary>
        public ArgValidateEvent<T> Validator { get { return m_Validator; } }
    }

    /// <summary>
    /// Class for interpreting command line arguments where custom handling is not required
    /// </summary>
    public class Options
    {
        /// <summary>
        /// Internal mapping from long options to an Argument
        /// </summary>
        protected Dictionary<string, Argument> m_LongOptions = new Dictionary<string, Argument>();

        /// <summary>
        /// Internal mapping from short options to an Argument
        /// </summary>
        protected Dictionary<char, Argument> m_ShortOptions = new Dictionary<char, Argument>();

        /// <summary>
        /// Parsed options
        /// </summary>
        protected Dictionary<string, string> m_RawOptions = new Dictionary<string, string>();

        /// <summary>
        /// List of all arguments that could not be parsed
        /// </summary>
        protected List<string> m_Unparsed = null;

        /// <summary>
        /// Constructor, parsing options from the command line
        /// </summary>
        /// <param name="arguments">List of arguments defining what's supported</param>
        /// <param name="args">Options provided by the user on the command line</param>
        public Options(Argument[] arguments, string[] args)
        {
            if (arguments == null || arguments.Length == 0)
                throw new ArgumentException("Must provide an array of arguments", "arguments");

            // Generate an internal mapping for parsing our arguments
            foreach (Argument arg in arguments) {
                m_LongOptions.Add(arg.LongOption, arg);
                if (arg.ShortOption != (char)0) m_ShortOptions.Add(arg.ShortOption, arg);
            }

            // TODO: Parse the command line, calling "AddParsed" or "AddUnparsed" accordingly
        }

        /// <summary>
        /// Used to add a new option to our list of options parsed
        /// </summary>
        /// <param name="argument">The long argument that was parsed</param>
        /// <param name="parameter">The (optional) parameter for the argument. Null indicates no parameter was provided</param>
        protected virtual void AddParsed(string longOption, string parameter)
        {
            Argument arg;
            if (m_LongOptions.TryGetValue(longOption, out arg))
                throw new ApplicationException("Internal exception, handling argument that isn't defined");

            int result;
            if (arg.ArgType == ArgType.Int) {
                if (!int.TryParse(parameter, out result)) {
                    // TODO: This should be our own exception
                    throw new ArgumentException("Argument: " + longOption + " is not an integer");
                }
            }

            if (m_RawOptions.ContainsKey(longOption)) {
                m_RawOptions[longOption] = parameter;
            } else {
                m_RawOptions.Add(longOption, parameter);
            }
        }

        /// <summary>
        /// Add an argument to the list of unparsed arguments
        /// </summary>
        /// <param name="argument">The argument string that wasn't parsed</param>
        protected void AddUnparsed(string argument)
        {
            if (m_Unparsed == null) {
                m_Unparsed = new List<string>();
            }
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
    public class Options<T>:Options where T : class,new()
    {
        /// <summary>
        /// List of options based on custom events
        /// </summary>
        private T m_UserOptions = null;

        /// <summary>
        /// Parse command line arguments given a definition of the arguments we should understand
        /// </summary>
        /// <param name="arguments">List of arguments defined for the program</param>
        /// <param name="args">Arguments provided on the command line</param>
        public Options(Argument[] arguments, string[] args)
            :base(arguments, args) { }

        protected override void AddParsed(string longOption, string parameter)
        {
            Argument arg;
            if (base.m_LongOptions.TryGetValue(longOption, out arg))
                throw new ApplicationException("Internal exception, handling argument that isn't defined");

            if (arg.ArgType == ArgType.Custom) {
                OnArgValidate(longOption, parameter);
            }

            base.AddParsed(longOption, parameter);
        }

        /// <summary>
        /// Call the user handler for parsing an argument
        /// </summary>
        /// <param name="longOption">The long option</param>
        /// <param name="parameter">The argument provided on the command line</param>
        protected void OnArgValidate(string longOption, string parameter)
        {
            if (m_UserOptions == null) {
                m_UserOptions = new T();
            }

            Argument arg;
            if (!base.m_LongOptions.TryGetValue(longOption, out arg)) 
                throw new ApplicationException("Internal exception, handling argument that isn't defined");

            // The base class only knows about "Argument", but it is actually of type "Argument<T>"
            ((Argument<T>)arg).Validator(this, new ArgValidatorArgs<T>(longOption, parameter, m_UserOptions));
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
