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
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u5}] {Message:lj}{NewLine}{Exception}"
                )
                .MinimumLevel.Verbose()
                .CreateLogger();

            if (arguments.Length != 3)
            {
                Console.WriteLine("o_O");
                return;
            }

            string readPath = arguments[0];
            string writePath = arguments[1];
            int.TryParse(arguments[2], out int speed);

            ReadableDevice device = new(readPath);
            WritableDevice uiDevice = new(writePath);

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
                await foreach (InputEvent evt in device.ReadInputEventsAsync([EventType.Key], token: cts.Token))
                {
                    if (evt.Type == (uint)EventType.Key)
                    {
                        if (evt.Value == 2)
                            switch (evt.Code)
                            {
                                // up
                                case 103:
                                    uiDevice.WriteEvents([new InputEvent(EventType.Relative, 0x01, -speed)]);
                                    break;
                                // down
                                case 108:
                                    uiDevice.WriteEvents([new InputEvent(EventType.Relative, 0x01, speed)]);
                                    break;
                                // left
                                case 105:
                                    uiDevice.WriteEvents([new InputEvent(EventType.Relative, 0x00, -speed)]);
                                    break;
                                // right
                                case 106:
                                    uiDevice.WriteEvents([new InputEvent(EventType.Relative, 0x00, speed)]);
                                    break;
                            }
                    }
                }
            });

            while (!cts.Token.IsCancellationRequested)
            {
                Thread.Sleep(100);
            }

            device.Dispose();
        }
    }
}
