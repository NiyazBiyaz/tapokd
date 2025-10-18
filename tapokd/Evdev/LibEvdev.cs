using System.Runtime.InteropServices;

namespace tapokd.Evdev
{
    /// <summary>
    /// Wrapper for <i>libevdev</i> linux library.
    /// Reference: <see href="https://www.freedesktop.org/software/libevdev/doc/latest/modules.html"/>.
    /// </summary>
    /// <remarks>
    /// Class doesn't implements all API functionality.
    /// </remarks>
    public partial class LibEvdev
    {
        #region Initialization and setup

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_new", SetLastError = true)]
        internal static partial nint New();

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_new_from_fd", SetLastError = true)]
        internal static partial int NewFromFd(int fd, ref nint dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_free", SetLastError = true)]
        internal static partial void Free(nint dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_grab", SetLastError = true)]
        internal static partial int Grab(nint dev, GrabMode grab);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_set_fd", SetLastError = true)]
        internal static partial int SetFd(nint dev, int fd);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_change-fd", SetLastError = true)]
        internal static partial int ChangeFd(nint dev, int fd);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_get_fd", SetLastError = true)]
        internal static partial int GetFd(nint dev);

        #endregion

        // Logging was skipped...

        #region  Querying device capabilities

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_get_name", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial string GetName(nint dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_get_phys", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial string GetPhys(nint dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_get_uniq", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial string GetUniq(nint dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_get_id_product", SetLastError = true)]
        internal static partial int GetIdProduct(nint dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_get_id_vendor", SetLastError = true)]
        internal static partial int GetIdVendor(nint dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_get_id_bustype", SetLastError = true)]
        internal static partial int GetIdBustype(nint dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_get_id_version", SetLastError = true)]
        internal static partial int GetIdVersion(nint dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_get_driver_version", SetLastError = true)]
        internal static partial int GetDriverVersion(nint dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_has_property", SetLastError = true)]
        internal static partial int HasProperty(nint dev, uint prop);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_has_event_type", SetLastError = true)]
        internal static partial int HasEventType(nint dev, uint type);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_has_event_code", SetLastError = true)]
        internal static partial int HasEventCode(nint dev, uint type, uint code);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_fetch_event_value", SetLastError = true)]
        internal static partial int FetchEventValue(nint dev, uint type, uint code, ref int value);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_get_repeat", SetLastError = true)]
        internal static partial int GetRepeat(nint dev, ref int delay, ref int period);

        // Multitouch was skipped...

        #endregion

        #region Modifying the appearance or capabilities of the device

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_set_name", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial void SetName(nint dev, string name);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_set_phys", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial void SetPhys(nint dev, string phys);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_set_uniq", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial void SetUniq(nint dev, string uniq);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_set_id_product", SetLastError = true)]
        internal static partial void SetIdProduct(nint dev, int product_id);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_set_id_vendor", SetLastError = true)]
        internal static partial void SetIdVendor(nint dev, int vendor_id);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_set_id_bustype", SetLastError = true)]
        internal static partial void SetIdBustype(nint dev, int bustype);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_set_id_version", SetLastError = true)]
        internal static partial void SetIdVersion(nint dev, int version);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_enable_property", SetLastError = true)]
        internal static partial int EnableProperty(nint dev, uint prop);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_disable_property", SetLastError = true)]
        internal static partial int DisableProperty(nint dev, uint prop);

        // Multitouch & Abs-related was skipped...

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_set_event_value", SetLastError = true)]
        internal static partial int SetEventValue(nint dev, uint type, uint code, int value);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_set_slot_value", SetLastError = true)]
        internal static partial int SetSlotValue(nint dev, uint slot, uint code, int value);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_enable_event_type", SetLastError = true)]
        internal static partial int EnableEventType(nint dev, uint type);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_disable_event_type", SetLastError = true)]
        internal static partial int DisableEventType(nint dev, uint type);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_enable_event_code", SetLastError = true)]
        internal static partial int EnableEventCode(nint dev, uint type, uint code);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_disable_event_code", SetLastError = true)]
        internal static partial int DisableEventCode(nint dev, uint type, uint code);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_kernel_set_led_value", SetLastError = true)]
        internal static partial int KernelSetLedValue(nint dev, uint code, LedValue value);

        #endregion

        #region  Miscellaneous helper functions

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_event_type_get_name", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial string? EventTypeGetName(uint type);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_event_code_get_name", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial string? EventCodeGetName(uint type, uint code);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_event_type_from_name_n", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial int EventTypeFromNameN(string name, nuint len);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_event_code_from_name_n", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial int EventCodeFromNameN(uint type, string name, nuint len);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_event_type_from_code_name_n", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial int EventTypeFromCodeNameN(string name, nuint len);

        #endregion

        #region Event handling

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_next_event", SetLastError = true)]
        internal static partial ReadStatus NextEvent(nint dev, ReadFlag flags, ref InputEvent ev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_has_event_pending", SetLastError = true)]
        internal static partial int HasEventPending(nint dev);

        #endregion

        #region Uinput device creation

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_uinput_create_from_device", SetLastError = true)]
        internal static partial int UinputCreateFromDevice(nint dev, int uinput_fd, ref nint uinput_dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_uinput_destroy", SetLastError = true)]
        internal static partial void UinputDestroy(nint uinput_dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_uinput_get_fd", SetLastError = true)]
        internal static partial int UinputGetFd(nint uinput_dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_uinput_get_syspath", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial string? UinputGetSysPath(nint uinput_dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_uinput_get_devnode", StringMarshalling = StringMarshalling.Utf8, SetLastError = true)]
        internal static partial string? UinputGetDevnode(nint uinput_dev);

        [LibraryImport("libevdev.so.2", EntryPoint = "libevdev_uinput_write_event", SetLastError = true)]
        internal static partial int UinputWriteEvent(nint uinput_dev, uint type, uint code, int value);

        #endregion
    }
}
