using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace AuroraDesignSuite.Services
{
    public static class LiveSyncBridge
    {
        public static void NotifyGameSync(string action = "REFRESH")
        {
            Task.Run(() =>
            {
                try
                {
                    using var client = new NamedPipeClientStream(".", "AuroraCommandSuiteSyncPipe", PipeDirection.Out);
                    client.Connect(150); // Fast non-blocking timeout
                    using var writer = new StreamWriter(client);
                    writer.WriteLine(action);
                    writer.Flush();
                }
                catch
                {
                    // Game is not running or pipe not active - silent failover
                }
            });
        }
    }
}
