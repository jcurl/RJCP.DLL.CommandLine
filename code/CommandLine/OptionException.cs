namespace RJCP.Core.CommandLine
{
    using System;
#if NETFRAMEWORK
    using System.Runtime.Serialization;
#endif

    /// <summary>
    /// A generic Option exception.
    /// </summary>
#if NETFRAMEWORK
    [Serializable]
#endif
    public class OptionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class.
        /// </summary>
        public OptionException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public OptionException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class with a specified error message and a
        /// reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception, or a <see langword="null"/> reference (Nothing in
        /// Visual Basic) if no inner exception is specified.
        /// </param>
        public OptionException(string message, Exception innerException) : base(message, innerException) { }

#if NETFRAMEWORK
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionException"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        protected OptionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
