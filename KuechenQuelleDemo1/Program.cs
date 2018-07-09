using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KuechenQuelleDemo1
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var utf8 = new UTF8Encoding(false);
            Console.InputEncoding = utf8;
            Console.OutputEncoding = utf8;

            var listener = new HttpListener();
            var appBases = new[] { "http://+:80/", $"http://+:80/Temporary_Listen_Addresses/{typeof(Program).Assembly.GetName().Name}/" };
            foreach (var appBase in appBases)
            {
                try
                {
                    listener.Prefixes.Clear();
                    listener.Prefixes.Add(appBase);
                    listener.Start();
                    break;
                }
                catch
                {
                    await Console.Error.WriteLineAsync($"Cannot bind to prefix {appBase}!");
                }                
            }

            await Console.Error.WriteLineAsync($"App server started at {string.Join(",", listener.Prefixes)} using endpoints: {string.Join(",", GetServerIPs())}");

            Console.CancelKeyPress += (pSender, pArgs) =>
            {
                listener.Stop();
                listener.Close();

                Environment.Exit(0);
            };

            await Task.Delay(100);

            while (listener.IsListening)
            {
                var req = await HandleContextAsync(await listener.GetContextAsync());
                await Console.Out.WriteLineAsync(req);
            }
        }

        private static Task<string> HandleContextAsync(HttpListenerContext ctx)
        {
            var req = ctx.Request;
            var res = ctx.Response;
            var now = DateTime.UtcNow.ToString("r");

            res.SendChunked = false;
            res.ContentLength64 = 0;
            res.Headers.Add("Server", Environment.MachineName);
            res.Headers.Add("Date", now);

            if (req.HttpMethod == "GET")
            {
                res.StatusCode = 200;
            }
            else
            {
                res.StatusCode = 405;
            }

            var log = $"{req.RemoteEndPoint.Address} {req.UserAgent} {now} \"{req.HttpMethod} {req.RawUrl}\" { res.StatusCode} 0";

            res.Close();

            return Task.FromResult(log);
        }

        public static IEnumerable<string> GetServerIPs()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress address in ipHostInfo.AddressList)
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                    yield return address.ToString();
            }

            yield break;
        }
    }
}
