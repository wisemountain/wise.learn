using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NLog;
using MessagePack;
using MessagePack.Resolvers;

namespace LearnNet
{
    class Program
    {
        [MessagePackObject]
        public class MsgEcho : Msg
        {
            [Key(0)]
            public string Hello { get; set; }

            public MsgEcho()
            {
                Type = (uint)MsgInternal.End + 100;
            }
        }

        class Client
        {
            public int MessageCount { get; private set; }

            private Logger logger = LogManager.GetCurrentClassLogger();

            public Client()
            {
                MessageCount = 0;
            }

            public void OnConnected(Msg m)
            {
                var echo = new MsgEcho();
                m.Protocol.Subscribe(this, echo.Type, OnEcho);

                echo.Hello = $"Hello {MessageCount}";
                m.Protocol.Send(echo);
            }

            public void OnDisconnected(Msg m)
            {
            }

            private void OnEcho(Msg m)
            {
                ++MessageCount;

                var echo = new MsgEcho();
                echo.Hello = $"Hello {MessageCount}";
                m.Protocol.Send(echo);

                logger.Info($"Echo. {echo.Hello}");
            }
            
        }

        class Server
        {
            private Logger logger = LogManager.GetCurrentClassLogger();

            public void OnAccepted(Msg m)
            {

                m.Protocol.Subscribe(this, new MsgEcho().Type, OnEcho);
            }

            public void OnEcho(Msg m)
            {
                m.Protocol.Send(m);

                var echo = (MsgEcho)m;

                logger.Info($"Echo. {echo.Hello}");
            }
        }

        static void Main(string[] args)
        {
            MsgSerializerFactory.Instance().Set(new MsgEcho().Type, typeof(MsgEcho));

            if (args[0] == "client")
            {
                MsgPackNode node = new MsgPackNode();

                Client c1 = new Client();

                node.Connect(
                    "127.0.0.1:5000", 
                    c1, 
                    c1.OnConnected, 
                    c1.OnDisconnected);

                Client c2 = new Client();

                node.Connect(
                    "127.0.0.1:5000", 
                    c2, 
                    c2.OnConnected, 
                    c2.OnDisconnected);

                while ( c1.MessageCount < 1000 || c2.MessageCount < 1000 )
                {
                    node.Process();

                    Thread.Sleep(1);                    
                } 
            }
            else
            {
                Server server = new Server();

                MsgPackNode node = new MsgPackNode();

                node.Listen("127.0.0.1:5000", 100, server, server.OnAccepted);

                while (true)
                {
                    node.Process();

                    System.Threading.Thread.Sleep(1);
                }
            } 
        }
    }
}
