using System;
using System.IO;
using System.Net.Sockets;

namespace LearnNet
{
    public abstract class ProtocolTcp : IProtocol
    {
        private SessionTcp session;

        public SessionTcp Session {  get { return session;  } }

        public ProtocolTcp()
        {
        }

        public ProtocolTcp(Socket socket)
        {
            session = new SessionTcp(this, socket);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public Result Listen(string address, int backLog)
        {
            session = new SessionTcp(this);
            return session.Listen(address, backLog);
        }

        /// <summary>
        /// IProtocol.Connect 구현. 대상에 연결한다. 
        /// </summary>
        /// <param name="address">상대 주소. Tcp는 IP:port, Host:port 형식</param>
        /// <param name="connected">연결 시 호출할 콜백</param>
        /// <param name="disconnected">단선 시 호출할 콜백</param>
        public Result Connect(string address)
        {
            Contract.Assert(session == null);

            session = new SessionTcp(this);
            return session.Connect(address);
        }

        /// <summary>
        /// IProtocol.Disconnect 구현. 연결을 종료한다. 
        /// </summary>
        public void Disconnect()
        {
            Contract.Assert(session != null);
            session.Disconnect();
        }

        /// <summary>
        /// 바이트를 받는다.
        /// </summary>
        public abstract void OnReceived(MemoryStream stream);

        /// <summary>
        /// 연결 결과를 통지 받는다. 
        /// </summary>
        public abstract void OnConnected(Result result, string message);

        /// <summary>
        /// 연결을 받았음
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        public abstract void OnAccepted(object socket);

        /// <summary>
        /// 연결 종료 결과를 통지 받는다.
        /// </summary>
        public abstract void OnDisconnected(Result result, string message);
    }
}
