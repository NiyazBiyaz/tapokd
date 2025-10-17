using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Mono.Unix.Native;
using Serilog;

namespace tapokd.Evdev
{
    public class ReadableDevice : BaseDevice
    {
        [SetsRequiredMembers]
        public ReadableDevice(string path)
            : base(path, OpenFlags.O_RDONLY | OpenFlags.O_NONBLOCK)
        {
        }

        public async IAsyncEnumerable<InputEvent> ReadInputEventsAsync(int updatePeriod = 1000, [EnumeratorCancellation] CancellationToken token = default)
        {
            Pollfd[] pollfds = [new Pollfd() { fd = FileDescriptor, events = PollEvents.POLLIN }];
            InputEvent inputEvent = default;
            ReadStatus res = ReadStatus.Success;
            ReadFlag flag;
            Log.Information("Start polling events.");

            while (!token.IsCancellationRequested)
            {
                flag = ReadFlag.Normal;
                await waitForPollAsync(pollfds, updatePeriod, token).ConfigureAwait(false);
                Log.Debug("Can read events.");
                bool stop = false;
                while (!stop)
                {
                    res = NextEvent(Dev, flag, ref inputEvent);
                    Log.Debug("Result = {Result}.", res);

                    if (res == ReadStatus.Sync)
                    {
                        flag = ReadFlag.Sync;
                        Log.Warning("Sync is requested.");
                    }

                    else if (res == ReadStatus.Again)
                    {
                        Log.Debug("No events to read.");
                        stop = true;
                        continue;
                    }

                    else if ((int)res < 0 && res != ReadStatus.Again)
                        throw new AutoExternalException((int)res);

                    yield return inputEvent;
                }
            }
            token.ThrowIfCancellationRequested();
        }

        private static async Task waitForPollAsync(Pollfd[] pollfds, int updatePeriod, CancellationToken token)
        {
            await Task.Run(() =>
            {
                while (!token.IsCancellationRequested)
                {
                    int res = Syscall.poll(pollfds, updatePeriod);
                    if (res > 0)
                        return;
                    else if (res == 0)
                        continue;
                    else
                        throw new AutoExternalException();
                }
                token.ThrowIfCancellationRequested();
            }, token);
        }
    }
}
