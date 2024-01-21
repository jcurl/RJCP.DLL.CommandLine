namespace RJCP.Core.CommandLine
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using RJCP.Core.Resources;

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
        /// Initializes a new instance of the <see cref="OptionMissingException"/> class with a specified error message.
        /// </summary>
        /// <param name="options">The options missing.</param>
        public OptionMissingException(string options)
            : base(MissingOptionMessage(options))
        {
            Options = options;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionMissingException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="options">The options missing.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a <see langword="null"/> reference (Nothing in
        /// Visual Basic) if no inner exception is specified.
        /// </param>
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
        /// Initializes a new instance of the <see cref="OptionMissingException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
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
        /// When overridden in a derived class, sets the <see cref="SerializationInfo"/> with information about the
        /// exception.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
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
}
