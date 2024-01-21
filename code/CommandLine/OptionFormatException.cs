namespace RJCP.Core.CommandLine
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using RJCP.Core.Resources;

    /// <summary>
    /// An option takes a parameter, but that parameter couldn't be converted to the correct type.
    /// </summary>
    [Serializable]
    public class OptionFormatException : OptionException
    {
        private static string IncorrectFormatOptionMessage(string option)
        {
            return string.Format(CmdLineStrings.OptionIncorrectFormat, option);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException"/> class.
        /// </summary>
        public OptionFormatException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException"/> class with a specified error message.
        /// </summary>
        /// <param name="option">The option provided with the wrong format.</param>
        public OptionFormatException(string option)
            : base(IncorrectFormatOptionMessage(option))
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="option">The option provided with the wrong format.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a <see langword="null"/> reference (Nothing in
        /// Visual Basic) if no inner exception is specified.
        /// </param>
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
        public OptionFormatException(string option, string message)
            : base(message)
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="option">The option provided with the wrong format.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a <see langword="null"/> reference (Nothing in
        /// Visual Basic) if no inner exception is specified.
        /// </param>
        public OptionFormatException(string option, string message, Exception innerException)
            : base(message, innerException)
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionFormatException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected OptionFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Option = info.GetString("option");
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
