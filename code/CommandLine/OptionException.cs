namespace RJCP.Core.CommandLine
{
    using System;
    using System.Runtime.Serialization;

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
        ///  or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
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
        ///  or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionDuplicateException(string message, Exception innerException) : base(message, innerException) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionDuplicateException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
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
            : base("Unknown option '" + option + "'")
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionUnknownException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="option">The option provided on the command line that is unknown.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionUnknownException(string option, Exception innerException)
            : base("Unknown option '" + option + "'", innerException)
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
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionUnknownException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Option = info.GetString("option");
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> 
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize our new property, call the base
            info.AddValue("option", Option);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the option that was unknown
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
            : base("Missing argument for '" + option + "'")
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingArgumentException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="option">The option provided on the command line that is unknown.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionMissingArgumentException(string option, Exception innerException)
            : base("Missing argument for '" + option + "'", innerException)
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

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingArgumentException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionMissingArgumentException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Option = info.GetString("option");
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> 
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize our new property, call the base
            info.AddValue("option", Option);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the option that was unknown
        /// </summary>
        /// <value>The option that was unknown.</value>
        public string Option { get; private set; }
    }

    /// <summary>
    /// An option was specified on the command line and is missing a mandatory argument.
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
            : base("Missing argument for options '" + options + "'")
        {
            Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="options">The options missing.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionMissingException(string options, Exception innerException)
            : base("Missing argument for options '" + options + "'", innerException)
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
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionMissingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Options = info.GetString("option");
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> 
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize our new property, call the base
            info.AddValue("option", Options);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the option that was unknown
        /// </summary>
        /// <value>The option that was unknown.</value>
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
            : base("Option '" + option + "' provided multiple times")
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAssignedException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="option">The options missing.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionAssignedException(string option, Exception innerException)
            : base("Option '" + option + "' provided multiple times", innerException)
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
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionAssignedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Option = info.GetString("option");
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> 
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize our new property, call the base
            info.AddValue("option", Option);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the option that was unknown
        /// </summary>
        /// <value>The option that was unknown.</value>
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
            : base("Option '" + option + "' has incorrect format")
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException" /> class with a specified error
        ///  message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="option">The option provided with the wrong format.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionFormatException(string option, Exception innerException)
            : base("Option '" + option + "' has incorrect format", innerException)
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="option">The option provided with the wrong format.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        ///  or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public OptionFormatException(string option, string message, Exception innerException)
            : base(message, innerException)
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        protected OptionFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Option = info.GetString("option");
        }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" />
        /// with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> 
        ///  that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" />
        ///  that contains contextual information about the source or destination.</param>
        /// <PermissionSet>
        ///   <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///   <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize our new property, call the base
            info.AddValue("option", Option);
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// Gets the option that was unknown
        /// </summary>
        /// <value>The option that was unknown.</value>
        public string Option { get; private set; }
    }
}
