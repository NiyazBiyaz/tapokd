using System.Diagnostics;
using System.Runtime.InteropServices;
using Mono.Unix.Native;

namespace tapokd.Evdev
{
    public class Device : LibEvdev, IDisposable
    {
        private readonly nint dev;
        private SafeFileDescriptor fileDescriptor = null!;

        /// <summary>
        /// Create new device and create <see cref="FileDescriptor"/> manually.
        /// </summary>
        public Device()
        {
            dev = New();
        }

        /// <summary>
        /// Create <see cref="Device"/> instance from path.
        /// </summary>
        /// <param name="path"><c>/dev/input/eventX</c> like path.</param>
        /// <param name="flags">Open flags.</param>
        /// <param name="mode">New file permissions if <see cref="OpenFlags.O_CREAT"/> in <paramref name="flags"/> and file doesn't exists.</param>
        public Device(string path, OpenFlags flags, FilePermissions? mode = null)
        {
            dev = New();
            FileDescriptor = new SafeFileDescriptor(path, flags, mode);
        }

        /// <summary>
        /// OS file descriptor of opened device. Automatically closes on dispose.
        /// </summary>
        public required SafeFileDescriptor FileDescriptor
        {
            get => fileDescriptor;
            set
            {
                int fd = (int)value.DangerousGetHandle();

                int err;
                if (fileDescriptor is null)
                    err = SetFd(dev, fd);
                else
                    err = ChangeFd(dev, fd);

                if (err != 0)
                    throw new ExternalException(Marshal.GetLastPInvokeErrorMessage(), Marshal.GetLastPInvokeError());

                Debug.Assert(GetFd(dev) == fd);

                fileDescriptor = value;
            }
        }

        /// <summary>
        /// Device name.
        /// </summary>
        public string Name
        {
            get => GetName(dev);
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                SetName(dev, value);
            }
        }

        /// <summary>
        /// Device phys.
        /// </summary>
        public string Phys
        {
            get => GetPhys(dev);
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                SetPhys(dev, value);
            }
        }

        /// <summary>
        /// Device uniq.
        /// </summary>
        public string Uniq
        {
            get => GetUniq(dev);
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                SetUniq(dev, value);
            }
        }

        /// <summary>
        /// Device driver version.
        /// </summary>
        public int DriverVersion => GetDriverVersion(dev);

        /// <summary>
        /// Combined representation of the device id properties as dictionary.
        /// It's similar to
        /// <code>
        ///     GetIdBustype(dev);
        ///     GetIdVendor(dev);
        ///     GetIdProduct(dev);
        ///     GetIdVersion(dev);
        /// </code>
        /// </summary>
        /// <remarks>
        /// Allows to individually set different <see cref="Id"/> properties using dicts.
        /// </remarks>
        public Dictionary<IdProperty, int> Id
        {
            get
            {
                int bus = GetIdBustype(dev);
                int vdr = GetIdVendor(dev);
                int pro = GetIdProduct(dev);
                int ver = GetIdVersion(dev);
                return new()
                {
                    { IdProperty.Bustype, bus },
                    { IdProperty.Vendor,  vdr },
                    { IdProperty.Product, pro },
                    { IdProperty.Version, ver }
                };
            }
            set
            {
                if (value.TryGetValue(IdProperty.Bustype, out int bus))
                    SetIdBustype(dev, bus);
                if (value.TryGetValue(IdProperty.Vendor, out int vdr))
                    SetIdVendor(dev, vdr);
                if (value.TryGetValue(IdProperty.Product, out int pro))
                    SetIdProduct(dev, pro);
                if (value.TryGetValue(IdProperty.Version, out int ver))
                    SetIdVersion(dev, ver);
            }
        }

        /// <summary>
        /// Make this device available only for this process.
        /// Use carefully.
        /// </summary>
        /// <param name="mode"><see cref="GrabMode.Grab"/> or <see cref="GrabMode.UnGrab"/> device.</param>
        /// <exception cref="ExternalException">If the device wasn't successfully grabbed.</exception>
        public void Grab(GrabMode mode)
        {
            int errno = Grab(dev, mode);
            if (errno != 0)
                throw new AutoExternalException(errno);
        }

        /// <summary>
        /// Get event type name by its value.
        /// </summary>
        /// <param name="type">Event type value</param>
        /// <returns>Type name as string</returns>
        /// <exception cref="ExternalException">If received event type is invalid</exception>
        public static string GetEventTypeName(uint type) => EventTypeGetName(type) ?? throw new AutoExternalException();
        /// <summary>
        /// Get event code name by its value & type value.
        /// </summary>
        /// <param name="type">Value of event type</param>
        /// <param name="code">Value of event code</param>
        /// <returns>Code name as string</returns>
        /// <exception cref="ExternalException">If received event code is invalid</exception>
        public static string GetEventCodeName(uint type, uint code) => EventCodeGetName(type, code) ?? throw new AutoExternalException();

        /// <summary>
        /// Get event type value by its name.
        /// </summary>
        /// <param name="typeName">Event type name</param>
        /// <returns>Event type value</returns>
        /// <exception cref="ExternalException">If received event type name is invalid</exception>
        public static uint GetEventTypeByName(string typeName)
        {
            ArgumentNullException.ThrowIfNull(typeName, nameof(typeName));
            nuint len = (nuint)typeName.Length;
            int res = EventTypeFromNameN(typeName, len);
            if (res < 0)
                throw new AutoExternalException();
            return (uint)res;
        }

        /// <summary>
        /// Get event type value by code name of its type.
        /// </summary>
        /// <param name="codeName">Event code name</param>
        /// <returns>Event code value</returns>
        /// <exception cref="ExternalException">If received event code name is invalid</exception>
        public static uint GetEventTypeByCodeName(string codeName)
        {
            ArgumentNullException.ThrowIfNull(codeName, nameof(codeName));
            nuint len = (nuint)codeName.Length;
            int res = EventTypeFromCodeNameN(codeName, len);
            if (res < 0)
                throw new AutoExternalException();
            return (uint)res;
        }

        /// <summary>
        /// Get event code value by its name.
        /// </summary>
        /// <param name="type">Event type value of the code</param>
        /// <param name="codeName">Event code name</param>
        /// <returns>Event code value</returns>
        /// <exception cref="ExternalException">If received event code name or type value are invalid</exception>
        public static uint GetEventCodeByName(uint type, string codeName)
        {
            ArgumentNullException.ThrowIfNull(codeName, nameof(codeName));
            nuint len = (nuint)codeName.Length;
            int res = EventCodeFromNameN(type, codeName, len);
            if (res < 0)
                throw new AutoExternalException();
            return (uint)res;
        }

        /// <inheritdoc cref="GetEventCodeByName"/>
        /// <param name="typeName">Event code type name</param>
        /// <param name="codeName">Event code name</param>
        /// <exception cref="ArgumentNullException">If received strings are null.</exception>
        public static uint GetEventCodeByName(string typeName, string codeName)
        {
            ArgumentNullException.ThrowIfNull(typeName, nameof(typeName));
            ArgumentNullException.ThrowIfNull(codeName, nameof(codeName));
            uint type = GetEventTypeByName(typeName);
            return GetEventCodeByName(type, codeName);
        }

        /// <param name="type">Event type value.</param>
        /// <param name="code">Event code value.</param>
        /// <returns><see langword="true"/> if device supports event, otherwise <see langword="false"/>.</returns>
        public bool HasEvent(uint type, uint? code = null) => HasEventType(dev, type) == 1 && (code is null || HasEventCode(dev, type, (uint)code) == 1);

        /// <inheritdoc cref="HasEvent"/>
        /// <param name="typeName">Event type name.</param>
        /// <param name="codeName">Event code name.</param>
        public bool HasEvent(string typeName, string? codeName = null)
        {
            ArgumentNullException.ThrowIfNull(typeName);
            uint type = GetEventTypeByName(typeName);
            return HasEvent(type, codeName is null ? null : GetEventCodeByName(type, codeName));
        }

        public void Enable(uint type, uint? code = null)
        {
            int res;
            if (code is null)
                res = EnableEventType(dev, type);
            else
                res = EnableEventCode(dev, type, (uint)code);

            if (res != 0)
                throw new AutoExternalException();
        }

        public void Enable(string typeName, string? codeName = null)
        {
            ArgumentNullException.ThrowIfNull(typeName);
            uint type = GetEventTypeByName(typeName);
            Enable(type, codeName is null ? null : GetEventCodeByName(type, codeName));
        }

        public void Disable(uint type, uint? code = null)
        {
            int res;
            if (code is null)
                res = DisableEventType(dev, type);
            else
                res = DisableEventCode(dev, type, (uint)code);

            if (res != 0)
                throw new AutoExternalException();
        }

        public void SetLed(uint led, LedValue value)
        {
            int res = KernelSetLedValue(dev, led, value);
            if (res < 0)
                throw new AutoExternalException();
        }

        public void SetLed(string led, LedValue value)
        {
            SetLed(GetEventCodeByName("EV_LED", led), value);
        }

        internal InputEvent? NextEvent(ReadFlag flags)
        {
            InputEvent ev = default;
            ReadStatus res = NextEvent(dev, flags, ref ev);
            if (res == ReadStatus.Again)
                return null;
            if (res < 0)
                throw new AutoExternalException();
            return ev;
        }

        public void Disable(string typeName, string? codeName = null)
        {
            ArgumentNullException.ThrowIfNull(typeName);
            uint type = GetEventTypeByName(typeName);
            Disable(type, codeName is null ? null : GetEventCodeByName(type, codeName));
        }

        public void Dispose()
        {
            Free(dev);
            FileDescriptor.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
