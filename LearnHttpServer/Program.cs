using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EmbedIO;

namespace LearnHttpServer
{
    class Program
    {
        public static void Main(string[] args)
        {
            // var url = "http://localhost:9696/";
            var url = "http://10.95.1.236:9696/";
            if (args.Length > 0)
                url = args[0];

            // Our web server is disposable.
            using (var server = CreateWebServer(url))
            {
                // Once we've registered our modules and configured them, we call the RunAsync() method.
                server.RunAsync();

                System.Threading.Thread.Sleep(100000);
            }
        }

        // Create and configure our web server.
        private static WebServer CreateWebServer(string url)
        {
            var server = new WebServer(o => o
                    .WithUrlPrefix(url)
                    .WithMode(HttpListenerMode.EmbedIO))
                // First, we will configure our web server by adding Modules.
                .WithLocalSessionManager();

            // WebApiModule에 WebApiController롤 RegisterController로 추가 해서 등록 
            // 문자열 등도 잘 돌려줌. 

            // Listen for state changes.
            // server.StateChanged += (s, e) => $"WebServer New State - {e.NewState}".Info();

            return server;
        }
    } 
}
