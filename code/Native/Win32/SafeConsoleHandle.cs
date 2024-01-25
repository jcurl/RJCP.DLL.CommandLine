namespace RJCP.Core.Native.Win32
{
    using System;
    using System.Runtime.ConstrainedExecution;
    using System.Runtime.InteropServices;

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

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        protected override bool ReleaseHandle()
        {
            // Nothing to release.
            return true;
        }
    }
}
