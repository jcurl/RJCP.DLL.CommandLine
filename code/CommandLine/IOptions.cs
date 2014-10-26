﻿namespace RJCP.Core.CommandLine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Optional Interface IOptions that can be used to customise output
    /// </summary>
    public interface IOptions
    {
        /// <summary>
        /// Displays usage help in case of an error when parsing options
        /// </summary>
        void Usage();

        /// <summary>
        /// Called when missing options are detected.
        /// </summary>
        /// <param name="missingOptions">The list missing options.</param>
        void Missing(IList<string> missingOptions);

        /// <summary>
        /// Called when an option is invalid or causes an error/exception.
        /// </summary>
        /// <param name="option">The offending option.</param>
        void InvalidOption(string option);
    }
}