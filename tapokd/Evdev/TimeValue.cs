using System.Runtime.InteropServices;

namespace tapokd.Evdev
{
    /// <summary>
    /// Representation of time in native system. ABI reference: <see pref="bits/types/struct_timeval.h"/>
    /// </summary>
    /// <param name="Seconds">Time value in <i>Unix time format</i> as 64-bit signed-integer</param>
    /// <param name="Microseconds">
    /// Time value in microseconds starting from the integer value of Unix-format.
    /// Value range: <b>000 000 - 999 999</b>   
    /// </param>
    [StructLayout(LayoutKind.Sequential)]
    internal readonly record struct TimeValue(
        long Seconds,
        long Microseconds
    );
}
