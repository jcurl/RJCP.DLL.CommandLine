namespace RJCP.Core.CommandLine
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using Resources;

    /// <summary>
    /// A generic Option exception.
    /// </summary>
    [Serializable]
    public class OptionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        public OptionException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OptionException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a <see langword="null"/> reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// The same option was specified multiple times in the options class.
    /// </summary>
    [Serializable]
    public class OptionDuplicateException : OptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionDuplicateException"/> class.
        /// </summary>
        public OptionDuplicateException() {}

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionDuplicateException" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OptionDuplicateException(string message) : base(message) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionDuplicateException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a <see langword="null"/> reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionDuplicateException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionDuplicateException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context) {}
    }

    /// <summary>
    /// An option was provided on the command line that is not defined.
    /// </summary>
    [Serializable]
    public class OptionUnknownException : OptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionUnknownException"/> class.
        /// </summary>
        public OptionUnknownException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionUnknownException" /> class with a specified error message.
        /// </summary>
        /// <param name="option">The option provided on the command line that is unknown.</param>
        public OptionUnknownException(string option)
            : base(UnknownOptionMessage(option))
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionUnknownException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="option">The option provided on the command line that is unknown.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a <see langword="null"/> reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionUnknownException(string option, Exception innerException)
            : base(UnknownOptionMessage(option), innerException)
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionUnknownException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="option">The option provided on the command line that is unknown.</param>
        public OptionUnknownException(string option, string message) : base(message)
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionUnknownException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionUnknownException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Option = info.GetString("option");
        }

        private static string UnknownOptionMessage(string option)
        {
            return string.Format(CmdLineStrings.OptionUnknown, option);
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo" />
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize our new property, call the base
            info.AddValue("option", Option);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the option that was unknown.
        /// </summary>
        /// <value>The option that was unknown.</value>
        public string Option { get; private set; }
    }

    /// <summary>
    /// An option was specified on the command line and is missing a mandatory argument.
    /// </summary>
    [Serializable]
    public class OptionMissingArgumentException : OptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingArgumentException"/> class.
        /// </summary>
        public OptionMissingArgumentException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingArgumentException" /> class with a specified error message.
        /// </summary>
        /// <param name="option">The option provided on the command line that is unknown.</param>
        public OptionMissingArgumentException(string option)
            : base(MissingOptionMessage(option))
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingArgumentException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="option">The option provided on the command line that is unknown.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a <see langword="null"/> reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionMissingArgumentException(string option, Exception innerException)
            : base(MissingOptionMessage(option), innerException)
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingArgumentException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="option">The option provided on the command line that is unknown.</param>
        public OptionMissingArgumentException(string option, string message)
            : base(message)
        {
            Option = option;
        }

        private static string MissingOptionMessage(string option)
        {
            return string.Format(CmdLineStrings.OptionMissingArgument, option);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingArgumentException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionMissingArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Option = info.GetString("option");
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo" />
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize our new property, call the base
            info.AddValue("option", Option);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the option that was missing an argument.
        /// </summary>
        /// <value>The option that was missing an argument.</value>
        public string Option { get; private set; }
    }

    /// <summary>
    /// A mandatory option was not specified on the command line.
    /// </summary>
    [Serializable]
    public class OptionMissingException : OptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingException"/> class.
        /// </summary>
        public OptionMissingException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingException" /> class with a specified error message.
        /// </summary>
        /// <param name="options">The options missing.</param>
        public OptionMissingException(string options)
            : base(MissingOptionMessage(options))
        {
            Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="options">The options missing.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a <see langword="null"/> reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionMissingException(string options, Exception innerException)
            : base(MissingOptionMessage(options), innerException)
        {
            Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="options">The options missing.</param>
        public OptionMissingException(string options, string message)
            : base(message)
        {
            Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionMissingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Options = info.GetString("option");
        }

        private static string MissingOptionMessage(string option)
        {
            return string.Format(CmdLineStrings.OptionMissingOption, option);
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo" />
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize our new property, call the base
            info.AddValue("option", Options);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the option that was missing.
        /// </summary>
        /// <value>The option that was missing.</value>
        public string Options { get; private set; }
    }

    /// <summary>
    /// An option was specified on the command line multiple times and it's not a list type.
    /// </summary>
    [Serializable]
    public class OptionAssignedException : OptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAssignedException"/> class.
        /// </summary>
        public OptionAssignedException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAssignedException" /> class with a specified error message.
        /// </summary>
        /// <param name="option">The options missing.</param>
        public OptionAssignedException(string option)
            : base(DuplicateOptionMessage(option))
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAssignedException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="option">The options missing.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a <see langword="null"/> reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionAssignedException(string option, Exception innerException)
            : base(DuplicateOptionMessage(option), innerException)
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAssignedException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="option">The options missing.</param>
        public OptionAssignedException(string option, string message)
            : base(message)
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAssignedException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionAssignedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Option = info.GetString("option");
        }

        private static string DuplicateOptionMessage(string option)
        {
            return string.Format(CmdLineStrings.OptionMultipleTimes, option);
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo" />
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize our new property, call the base
            info.AddValue("option", Option);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the option that is specified multiple times.
        /// </summary>
        /// <value>The option that is specified multiple times.</value>
        public string Option { get; private set; }
    }

    /// <summary>
    /// An option takes a parameter, but that parameter couldn't be converted to the correct type.
    /// </summary>
    [Serializable]
    public class OptionFormatException : OptionException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException"/> class.
        /// </summary>
        public OptionFormatException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException" /> class with a specified error message.
        /// </summary>
        /// <param name="option">The option provided with the wrong format.</param>
        public OptionFormatException(string option)
            : base(IncorrectFormatOptionMessage(option))
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="option">The option provided with the wrong format.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a <see langword="null"/> reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionFormatException(string option, Exception innerException)
            : base(IncorrectFormatOptionMessage(option), innerException)
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="option">The option provided with the wrong format.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a <see langword="null"/> reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionFormatException(string option, string message, Exception innerException)
            : base(message, innerException)
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Option = info.GetString("option");
        }

        private static string IncorrectFormatOptionMessage(string option)
        {
            return string.Format(CmdLineStrings.OptionIncorrectFormat, option);
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="SerializationInfo" />
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize our new property, call the base
            info.AddValue("option", Option);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the option that is in an invalid format.
        /// </summary>
        /// <value>The option that is invalid.</value>
        public string Option { get; private set; }
    }
}
