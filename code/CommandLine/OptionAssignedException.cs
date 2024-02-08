namespace RJCP.Core.CommandLine
{
    using System;
    using Resources;
#if NETFRAMEWORK
    using System.Runtime.Serialization;
    using System.Security.Permissions;
#endif

    /// <summary>
    /// An option was specified on the command line multiple times and it's not a list type.
    /// </summary>
#if NETFRAMEWORK
    [Serializable]
#endif
    public class OptionAssignedException : OptionException
    {
        private static string DuplicateOptionMessage(string option)
        {
            return string.Format(CmdLineStrings.OptionMultipleTimes, option);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAssignedException"/> class.
        /// </summary>
        public OptionAssignedException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAssignedException"/> class with a specified error
        /// message.
        /// </summary>
        /// <param name="option">The options missing.</param>
        public OptionAssignedException(string option)
            : base(DuplicateOptionMessage(option))
        {
            Option = option;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAssignedException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="option">The options missing.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a <see langword="null"/> reference (Nothing in
        /// Visual Basic) if no inner exception is specified.
        /// </param>
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

#if NETFRAMEWORK
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionAssignedException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected OptionAssignedException(SerializationInfo info, StreamingContext context)
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
#endif

        /// <summary>
        /// Gets the option that is specified multiple times.
        /// </summary>
        /// <value>The option that is specified multiple times.</value>
        public string Option { get; private set; }
    }
}
