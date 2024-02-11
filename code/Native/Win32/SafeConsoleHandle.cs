namespace RJCP.Core.Native.Win32
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
#if NETFRAMEWORK
    using System.Runtime.ConstrainedExecution;
#endif

    [SupportedOSPlatform("windows")]
    internal class SafeConsoleHandle : SafeHandle
    {
        public SafeConsoleHandle() : base(IntPtr.Zero, false) { }

        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero;
            }
        }

#if NETFRAMEWORK
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
#endif
        protected override bool ReleaseHandle()
        {
            // Nothing to release.
            return true;
        }
    }
}
