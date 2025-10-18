using Microsoft.Win32.SafeHandles;
using Mono.Unix;
using Mono.Unix.Native;

namespace tapokd.Evdev
{
    public class SafeFileDescriptor : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SafeFileDescriptor(string path, OpenFlags openFlags, FilePermissions? filePermissions = null)
            : base(true)
        {
            int fd = filePermissions is not null ? Syscall.open(path, openFlags, (FilePermissions)filePermissions) : Syscall.open(path, openFlags);
            if (fd < 0)
                throw new UnixIOException(Stdlib.GetLastError());
            SetHandle(fd);
        }

        public SafeFileDescriptor(int fileDescriptor)
            : base(true)
        {
            SetHandle(fileDescriptor);
        }

        protected override bool ReleaseHandle()
        {
            int fd = (int)handle;
            if (!IsClosed)
                return Syscall.close(fd) == 0;
            return true;
        }
    }
}
