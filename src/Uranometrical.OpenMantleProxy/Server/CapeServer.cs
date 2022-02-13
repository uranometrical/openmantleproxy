using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Uranometrical.OpenMantleProxy.Server
{
    public class CapeServer
    {
        protected readonly HttpListener Listener = new();
        protected readonly HttpClient Client = new();
        protected readonly string[] Servers;

        public CapeServer(params string[] servers)
        {
            // We want to listen to port 80 as that's the default HTTP port (used for s.optifine.net).
            Listener.Prefixes.Add("http://127.0.0.1:80/");
            Listener.Prefixes.Add("http://localhost:80/");

            Client.DefaultRequestHeaders.Add("User-Agent", "OpenMantleProxy Cape Server Client");

            Servers = servers;
        }

        public virtual void Listen()
        {
            Listener.Start();

            while (true)
            {
                HttpListenerContext context = Listener.GetContext();
                Console.WriteLine(context.Request.RawUrl);

                byte[] buf = ResolveCape(context.Request.RawUrl ?? "");

                if (buf == Array.Empty<byte>())
                {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                    continue;
                }
                
                HttpListenerResponse response = context.Response;
                response.ContentLength64 = buf.Length;
                response.OutputStream.Write(buf);

                context.Response.Close();
            }
        }

        protected virtual byte[] ResolveCape(string input)
        {
            foreach (string server in Servers)
            {
                HttpResponseMessage response = Client.GetAsync(server + input).Result;

                if (response.StatusCode == HttpStatusCode.NotFound)
                    continue;

                if (response.Content.ReadAsStringAsync().Result.Contains("Not Found"))
                    continue;

                if (response.Content.Headers.ContentType?.ToString() != "image/png")
                    continue;

                using Stream stream = response.Content.ReadAsStream();
                return stream is not MemoryStream memory ? Encoding.UTF8.GetBytes("404 uh oh") : memory.ToArray();
            }

            return Array.Empty<byte>();
        }
    }
}