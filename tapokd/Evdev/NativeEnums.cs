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
        Managed = -2,
    }

    // API reference: linux/input-event-codes.h
    public enum EventType : uint
    {
        Synchronization = 0x00,
        Key = 0x01,
        Relative = 0x02,
        Absolute = 0x03,
        Miscellaneous = 0x04,
        Switch = 0x05,
        Led = 0x11,
        Sounds = 0x12,
        Replay = 0x14,
        ForceFeedback = 0x15,
        Power = 0x16,
        ForceFeedbackStatus = 0x17,
        Maximum = 0x1f,
        Count = Maximum + 1
    }
}
