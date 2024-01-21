namespace RJCP.Core.CommandLine
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using RJCP.Core.Resources;

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
        /// Initializes a new instance of the <see cref="OptionUnknownException"/> class with a specified error message.
        /// </summary>
        /// <param name="option">The option provided on the command line that is unknown.</param>
        public OptionUnknownException(string option)
            : base(UnknownOptionMessage(option))
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionUnknownException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="option">The option provided on the command line that is unknown.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a <see langword="null"/> reference (Nothing in
        /// Visual Basic) if no inner exception is specified.
        /// </param>
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
        /// Initializes a new instance of the <see cref="OptionUnknownException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected OptionUnknownException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Option = info.GetString("option");
        }

        private static string UnknownOptionMessage(string option)
        {
            return string.Format(CmdLineStrings.OptionUnknown, option);
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
        /// Gets the option that was unknown.
        /// </summary>
        /// <value>The option that was unknown.</value>
        public string Option { get; private set; }
    }
}
