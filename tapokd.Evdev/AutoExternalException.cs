using System.Runtime.InteropServices;

namespace tapokd.Evdev
{
    /// <summary>
    /// <see cref="ExternalException"/> with automatic insertion of <see cref="Marshal.GetLastPInvokeErrorMessage"/> and <see cref="Marshal.GetLastPInvokeError"/>
    /// </summary>
    public static class AutoExternalException
    {
        public static ExternalException Throw() => new(Marshal.GetLastPInvokeErrorMessage(), Marshal.GetLastPInvokeError());

        public static ExternalException Throw(int errorCode) => new(Marshal.GetLastPInvokeErrorMessage(), errorCode);
    }
}
