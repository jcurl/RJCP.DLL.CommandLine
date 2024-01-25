namespace RJCP.Core.Native.Win32
{
    using System.Runtime.InteropServices;
    using System.Security;

    [SuppressUnmanagedCodeSecurity]
    internal static class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern bool GetConsoleMode(SafeConsoleHandle hConsoleHandle, out int lpMode);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        public static extern SafeConsoleHandle GetConsoleWindow();
    }
}
