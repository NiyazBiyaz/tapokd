using Mono.Unix.Native;
using Serilog;
using tapokd.Evdev;

namespace tapokd
{
    public class Program
    {
        public static void Main(string[] arguments)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .MinimumLevel.Verbose()
                .CreateLogger();

            if (arguments.Length != 1)
            {
                Console.WriteLine("o_O");
                return;
            }

            string path = arguments.First();

            ReadableDevice device = new(path);

            Console.WriteLine(device.Name);
            Console.WriteLine(Syscall.getpid());

            var cts = new CancellationTokenSource();

            Console.CancelKeyPress += new((_, _) =>
            {
                Log.Verbose("Stop working");
                cts.Cancel();
            });

            Task.Run(async () =>
            {
                await foreach (InputEvent evt in device.ReadInputEventsAsync(token: cts.Token))
                    Log.Debug("{InputEvent}", evt);
            });

            while (!cts.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            device.Dispose();
        }
    }
}
