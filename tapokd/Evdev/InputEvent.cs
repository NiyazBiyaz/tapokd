using System.Runtime.InteropServices;

namespace tapokd.Evdev
{
    #region Structs

    /// <summary>
    /// Input event structure from <see href="linux/input.h"/>.
    /// </summary>
    /// <param name="TimeValue">Time of event receiving.</param>
    /// <param name="Type">Event type.</param>
    /// <param name="Code">Event code.</param>
    /// <param name="Value">Event value.</param>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct InputEvent(
        TimeValue TimeValue,
        ushort Type,
        ushort Code,
        int Value
    );

    #endregion
}
