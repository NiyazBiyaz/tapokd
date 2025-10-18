using Mono.Unix.Native;
using Serilog;

namespace tapokd.Evdev
{
    public class WritableDevice : BaseDevice, IDisposable
    {
        private readonly nint uiDev;

        public WritableDevice(string path)
        {
            SetFileDescriptor(new SafeFileDescriptor(path, OpenFlags.O_RDWR | OpenFlags.O_NONBLOCK));

            int err = UinputCreateFromDevice(Dev, (int)UinputOpenMode.Managed, ref uiDev);
            if (err != 0)
                throw AutoExternalException.Throw(err);
        }

        public void WriteEvents(InputEvent[] inputEvents, bool strict = true)
        {
            ArgumentNullException.ThrowIfNull(inputEvents, nameof(inputEvents));

            int err;
            foreach (var evt in inputEvents)
            {
                Log.Debug("Writing event: {Event}", evt);
                err = UinputWriteEvent(uiDev, evt.Type, evt.Code, evt.Value);
                if (err < 0)
                    if (strict)
                        throw AutoExternalException.Throw(-err);
                    else
                        Log.Error("Error has been occurred: {ErrorCode}", err);
            }
            err = UinputWriteEvent(uiDev, (uint)EventType.Synchronization, 0, 0);
        }

        public override void Dispose()
        {
            UinputDestroy(uiDev);
            base.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
