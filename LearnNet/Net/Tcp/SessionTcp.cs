using System;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using NLog;

namespace LearnNet
{
    /// <summary>
    /// Session Async Event Args 
    /// </summary>
    public class SessionTcp
    {
        private ProtocolTcp protocol;
        private string host;
        private ushort port;

        private Socket listenSocket;
        private Socket socket;
        private const int recvBufferSize = 32*1024;
        private byte[] recvBuffer = new byte[recvBufferSize];

        private SocketAsyncEventArgs acceptEventArgs;
        private SocketAsyncEventArgs connectEventArgs;
        private SocketAsyncEventArgs rxEventArgs;
        private SocketAsyncEventArgs txEventArgs;

        private List<ArraySegment<byte>> rxList = new List<ArraySegment<byte>>(); 
        private MemoryStream recvStream = new MemoryStream();
        private volatile Int32 sending = 0;
        private object lockSendStream = new object();
        private MemoryStream sendStream1 = new MemoryStream();
        private MemoryStream sendStream2 = new MemoryStream();
        private MemoryStream accumulStream;
        private MemoryStream sendStream;

        private volatile bool activeClose = false;

        Logger logger = LogManager.GetCurrentClassLogger();

        public SessionTcp(ProtocolTcp protocol)
        {
            this.protocol = protocol;
            this.accumulStream = sendStream1;

            acceptEventArgs = new SocketAsyncEventArgs();
            connectEventArgs = new SocketAsyncEventArgs();
            rxEventArgs = new SocketAsyncEventArgs();
            txEventArgs = new SocketAsyncEventArgs();

            acceptEventArgs.Completed += OnAcceptCompleted;
            connectEventArgs.Completed += OnConnectCompleted;
            rxEventArgs.Completed += OnRecvCompleted;
            txEventArgs.Completed += OnSendCompleted;

            rxList.Add(new ArraySegment<byte>(recvBuffer));
        }

        /// <summary>
        /// 외부에서 소켓을 제공해주는 생성자. 수신을 시작한다. 
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="socket"></param>
        public SessionTcp(ProtocolTcp protocol, Socket socket)
            : this(protocol)
        {
            this.socket = socket;
        }

        public void BeginRecvInternal()
        {
            RequestRecv();
        }

        public Result Listen(string address, int backLog)
        {
            string h;
            ushort p;

            ParseAddress(address, out h, out p);

            IPAddress ipAddress = Dns.GetHostAddresses(h)[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, p);
            
            listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(localEndPoint);
            listenSocket.Listen(backLog);

            RequestAccept();

            return Result.Success;
        }

        public Result Connect(string address)
        {
            Contract.Assert(socket == null);
            Contract.Assert(protocol != null);

            ParseAddress(address, out host, out port);

            // XXX: GetHostAddress(host)[0]는 대부분의 경우 괜찮겠지만 확실한 방법은 아니다. 

            IPAddress ipAddress = Dns.GetHostAddresses(host)[0];
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ConnectInternal(socket, remoteEP);

            return Result.Success;
        }

        public bool IsConnected()
        {
            return socket != null; // socket.Closed가 false인 경우가 있다. 
        }

        public void Disconnect()
        {
            activeClose = true;

            DisconnectInternal("Disconnected by Application");
        }

        public Result Send(byte[] payload)
        {
            return Send(payload, 0, payload.Length);
        }

        public Result Send(byte[] payload, int offset, int length)
        {
            lock (lockSendStream)
            {

                accumulStream.Write(payload, offset, length);
            }

            RequestSend();

            return Result.Success;
        }

        void RequestAccept()
        {
            try
            {
                logger.Trace("RequestAccept.");

                acceptEventArgs.AcceptSocket = null;
            
                bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArgs);
                if (!willRaiseEvent)
                {
                    OnAcceptCompleted(null, acceptEventArgs);
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception e)
            {
                Fail($"recv error {e}");
            }
        }

        void RequestRecv()
        {
            try
            {
                rxEventArgs.SetBuffer(recvBuffer, 0, recvBufferSize);

                bool pending = socket.ReceiveAsync(rxEventArgs);
                if (!pending)
                {
                    OnRecvCompleted(socket, rxEventArgs);
                }
            }
            catch (ObjectDisposedException) { }
            catch (Exception e)
            {
                Fail($"recv error {e}");
            }
        } 

        void RequestSend()
        {
            if ( Interlocked.Exchange(ref sending, 1) == 1)
            {
                return;
            }

            // sending == 1 : Exchange set it to 1

            if (accumulStream.Position == 0)
            {
                Interlocked.Exchange(ref sending, 0);

                return;
            }

            SwitchSendStream();

            try
            {
                var buf = sendStream.GetBuffer();

                logger.Trace($"Sending. {sendStream.Position}");

                txEventArgs.SetBuffer(buf, 0, (int)sendStream.Position);

                bool pending = socket.SendAsync(txEventArgs);
                if (!pending)
                {
                    OnSendCompleted(socket, txEventArgs);
                }
            }
            catch ( Exception ex)
            {
                if (IsConnected()) // recv 쪽에서 끊어지면 Disposed 예외가 발생
                {
                    Fail($"session error {ex}");
                }

                Interlocked.Exchange(ref sending, 0);
            }
        }

        private void ConnectInternal(Socket socket, EndPoint endpoint)
        {
            try
            {
                if (Object.ReferenceEquals(socket, null))
                {
                    socket = new Socket(
                        endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                }

                connectEventArgs.RemoteEndPoint = endpoint;
                connectEventArgs.UserToken = socket;

                bool pending = socket.ConnectAsync(connectEventArgs);

                if (!pending)
                {
                    OnConnectCompleted(socket, connectEventArgs);
                }
            }
            catch (Exception e)
            {
                // connected 콜백으로 에러를 보낸다. 
                protocol.OnConnected(Result.Fail, $"Error connecting to {endpoint} : {e.Message}");
            }
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            protocol.OnAccepted(e.AcceptSocket);

            RequestAccept();
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            logger.Trace( $"Connected to {host}:{port}");

            var socket = (Socket)e.UserToken;

            if (e.SocketError == SocketError.Success)
            {
                protocol.OnConnected(Result.Success, $"Connected to {host}:{port}");

                // TODO: 아래 주석 내용 확인
                // connectEventArgs.Completed -= OnConnectCompleted;
                // connectEventArgs.Dispose();
                // connectEventArgs = null;

                RequestRecv();
            }
            else
            {
                protocol.OnConnected(Result.Fail, $"{e.RemoteEndPoint} error connecting to {e.SocketError}");
            }
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                switch (e.SocketError)
                {
                    case SocketError.Success:
                        break;
                    case SocketError.OperationAborted:
                    default:
                        Fail($"Error: {e.SocketError}");
                        return;
                }
                
                var bytesRead = e.BytesTransferred;

                if ( bytesRead == 0)
                { 
                    Fail($"disconnected by peer.");
                    return;
                }

                if (bytesRead > 0)
                {
                    logger.Trace($"Recv. Bytes: {bytesRead} Stream: {recvStream.Position}");

                    recvStream.Write(recvBuffer, 0, bytesRead);

                    protocol.OnReceived(recvStream);

                    // 앞으로 이동 시킴
                    recvStream.Seek(0, SeekOrigin.Begin); 
                }

                RequestRecv();
            }
            catch (Exception ex)
            {
                Fail($"session error {ex}");
            }
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs e)
        {
            Interlocked.Exchange(ref sending, 0);

            try
            {
                switch (e.SocketError)
                {
                    case SocketError.Success:
                        {
                            lock (lockSendStream)
                            {
                                // 처음으로 돌림
                                sendStream.Seek(0, SeekOrigin.Begin);
                            }

                            RequestSend();
                        }
                        return;
                    case SocketError.OperationAborted:
                        // disconnect from server. OnRecvCompleted detects it. 
                        return;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Fail($"session error {ex}");
            }
        }

        void SwitchSendStream()
        {
            lock (lockSendStream)
            {
                if (Object.ReferenceEquals(accumulStream, sendStream1))
                {
                    accumulStream = sendStream2;
                    sendStream = sendStream1;
                }
                else
                {
                    accumulStream = sendStream1;
                    sendStream = sendStream2;
                }
            }
        }

        private void Fail(string msg)
        {
            logger.Warn(msg);

            DisconnectInternal(msg);
        }

        private void DisconnectInternal(string msg)
        {
            if (IsConnected())
            {
                socket.Close();
                socket.Dispose();
                socket = null;
                connectEventArgs.Dispose();
                txEventArgs.Dispose();
                rxEventArgs.Dispose();

                if (activeClose)
                {
                    protocol.OnDisconnected(Result.Success_ActiveClose, msg);
                }
                else
                {
                    protocol.OnDisconnected(Result.Fail, msg);
                }
            }
        }

        private void ParseAddress(string address, out string h, out ushort p)
        {
            h = "";
            p = 0;

            string[] s = address.Split(':'); 

            if ( s.Length >= 2)
            {
                h = s[0];
                p = ushort.Parse(s[1]);
            }
        }
    }
}
