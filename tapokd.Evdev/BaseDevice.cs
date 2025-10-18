using System.Diagnostics;
using System.Runtime.InteropServices;
using Mono.Unix.Native;

namespace tapokd.Evdev
{
    public abstract class BaseDevice : LibEvdev, IDisposable
    {
        protected readonly nint Dev;
        private SafeFileDescriptor fileDescriptorHandle = null!;

        public BaseDevice(int fileDescriptor)
        {
            Dev = New();
            SetFileDescriptor(new SafeFileDescriptor(fileDescriptor));
        }

        /// <summary>
        /// Create <see cref="BaseDevice"/> instance from path.
        /// </summary>
        /// <param name="path"><c>/dev/input/eventX</c> like path.</param>
        /// <param name="flags">Open flags.</param>
        /// <param name="mode">New file permissions if <see cref="OpenFlags.O_CREAT"/> in <paramref name="flags"/> and file doesn't exists.</param>
        public BaseDevice(string path, OpenFlags flags, FilePermissions? mode = null)
        {
            Dev = New();
            SetFileDescriptor(new SafeFileDescriptor(path, flags, mode));
        }

        protected BaseDevice()
        {
            Dev = New();
        }

        protected void SetFileDescriptor(SafeFileDescriptor fileDescriptor)
        {
            int fd = (int)fileDescriptor.DangerousGetHandle();

            int err;
            if (fileDescriptorHandle is null)
                err = SetFd(Dev, fd);
            else
                err = ChangeFd(Dev, fd);

            if (err != 0)
                throw new ExternalException(Marshal.GetLastPInvokeErrorMessage(), Marshal.GetLastPInvokeError());

            Debug.Assert(GetFd(Dev) == fd);

            fileDescriptorHandle = fileDescriptor;
        }

        /// <summary>
        /// File descriptor of the opened device.
        /// </summary>
        public int FileDescriptor => (int)fileDescriptorHandle.DangerousGetHandle();

        /// <summary>
        /// Device name.
        /// </summary>
        public string Name
        {
            get => GetName(Dev);
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                SetName(Dev, value);
            }
        }

        /// <summary>
        /// Device phys.
        /// </summary>
        public string Phys
        {
            get => GetPhys(Dev);
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                SetPhys(Dev, value);
            }
        }

        /// <summary>
        /// Device uniq.
        /// </summary>
        public string Uniq
        {
            get => GetUniq(Dev);
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                SetUniq(Dev, value);
            }
        }

        /// <summary>
        /// Device driver version.
        /// </summary>
        public int DriverVersion => GetDriverVersion(Dev);

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
                int bus = GetIdBustype(Dev);
                int vdr = GetIdVendor(Dev);
                int pro = GetIdProduct(Dev);
                int ver = GetIdVersion(Dev);
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
                    SetIdBustype(Dev, bus);
                if (value.TryGetValue(IdProperty.Vendor, out int vdr))
                    SetIdVendor(Dev, vdr);
                if (value.TryGetValue(IdProperty.Product, out int pro))
                    SetIdProduct(Dev, pro);
                if (value.TryGetValue(IdProperty.Version, out int ver))
                    SetIdVersion(Dev, ver);
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
            int errno = Grab(Dev, mode);
            if (errno != 0)
                throw AutoExternalException.Throw(errno);
        }

        /// <summary>
        /// Get event type name by its value.
        /// </summary>
        /// <param name="type">Event type value</param>
        /// <returns>Type name as string</returns>
        /// <exception cref="ExternalException">If received event type is invalid</exception>
        public static string GetEventTypeName(uint type) => EventTypeGetName(type) ?? throw AutoExternalException.Throw();
        /// <summary>
        /// Get event code name by its value & type value.
        /// </summary>
        /// <param name="type">Value of event type</param>
        /// <param name="code">Value of event code</param>
        /// <returns>Code name as string</returns>
        /// <exception cref="ExternalException">If received event code is invalid</exception>
        public static string GetEventCodeName(uint type, uint code) => EventCodeGetName(type, code) ?? throw AutoExternalException.Throw();

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
                throw AutoExternalException.Throw();
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
                throw AutoExternalException.Throw();
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
                throw AutoExternalException.Throw();
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
        public bool HasEvent(uint type, uint? code = null) => HasEventType(Dev, type) == 1 && (code is null || HasEventCode(Dev, type, (uint)code) == 1);

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
                res = EnableEventType(Dev, type);
            else
                res = EnableEventCode(Dev, type, (uint)code);

            if (res != 0)
                throw AutoExternalException.Throw();
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
                res = DisableEventType(Dev, type);
            else
                res = DisableEventCode(Dev, type, (uint)code);

            if (res != 0)
                throw AutoExternalException.Throw();
        }

        public void Disable(string typeName, string? codeName = null)
        {
            ArgumentNullException.ThrowIfNull(typeName);
            uint type = GetEventTypeByName(typeName);
            Disable(type, codeName is null ? null : GetEventCodeByName(type, codeName));
        }

        public void SetLed(uint led, LedValue value)
        {
            int res = KernelSetLedValue(Dev, led, value);
            if (res < 0)
                throw AutoExternalException.Throw();
        }

        public void SetLed(string led, LedValue value)
        {
            SetLed(GetEventCodeByName("EV_LED", led), value);
        }

        public virtual void Dispose()
        {
            Free(Dev);
            fileDescriptorHandle.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
