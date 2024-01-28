namespace RJCP.Core.Terminal
{
    using System;
    using Native.Win32;
    using RJCP.Core.Environment;

    /// <summary>
    /// A formatter class that knows about the console.
    /// </summary>
    internal class ConsoleFormat : Format
    {
        public ConsoleFormat()
        {
#if NET45_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            IsRedirected = Console.IsOutputRedirected && Console.IsErrorRedirected;
#else
            if (Platform.IsWinNT()) {
                SafeConsoleHandle handle = Kernel32.GetConsoleWindow();
                if (handle.IsInvalid || !Kernel32.GetConsoleMode(handle, out int _)) {
                    IsRedirected = true;
                }
            } else {
                try {
                    if (Console.BufferWidth == 0) {
                        IsRedirected = true;
                    }
                } catch (Exception) {
                    // Nothing to do. The console handle must be invalid to raise an exception, so it's being
                    // redirected.
                }
            }
#endif
        }

        /// <summary>
        /// Gets a value indicating whether the console is redirected.
        /// </summary>
        /// <value><see langword="true"/> if this console is redirected; otherwise, <see langword="false"/>.</value>
        public bool IsRedirected { get; }

        /// <summary>
        /// Gets the width of the terminal.
        /// </summary>
        /// <value>The width of the terminal.</value>
        public override int Width
        {
            get
            {
                if (!IsRedirected) return Console.BufferWidth;
                return 80;
            }
        }

        /// <summary>
        /// Gets the height of the terminal.
        /// </summary>
        /// <value>The height of the terminal.</value>
        public override int Height
        {
            get
            {
                if (!IsRedirected) return Console.WindowHeight;
                return 25;
            }
        }
    }
}
