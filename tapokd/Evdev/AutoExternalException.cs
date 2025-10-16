using System.Runtime.InteropServices;

namespace tapokd.Evdev
{
    /// <summary>
    /// <see cref="ExternalException"/> with automatic insertion of <see cref="Marshal.GetLastPInvokeErrorMessage"/> and <see cref="Marshal.GetLastPInvokeError"/>
    /// </summary>
    public sealed class AutoExternalException : ExternalException
    {
        public AutoExternalException()
            : base(Marshal.GetLastPInvokeErrorMessage(), Marshal.GetLastPInvokeError())
        {
        }

        public AutoExternalException(int errorCode)
            : base(Marshal.GetLastPInvokeErrorMessage(), errorCode)
        {
        }
    }

    #endregion
}
