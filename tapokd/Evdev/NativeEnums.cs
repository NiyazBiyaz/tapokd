using Mono.Unix.Native;

namespace tapokd.Evdev
{
    // API references: libevdev/libevdev.h
    public enum GrabMode
    {
        Grab = 3,
        UnGrab = 4,
    }

    public enum ReadFlag : uint
    {
        Sync = 1,
        Normal = 2,
        ForceSync = 4,
        Blocking = 8,
    }

    public enum ReadStatus
    {
        Success,
        Sync,
        Again = -Errno.EAGAIN
    }

    public enum LedValue
    {
        On = 3,
        Off = 4,
    }

    // API reference: libevdev/libevdev-uinput.h
    internal enum UinputOpenMode
    {
        OpenManaged = -2,
    }
}
