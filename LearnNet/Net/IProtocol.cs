using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace LearnNet 
{

    /// <summary>
    /// Bot에서 관리하기 위한 기본 인터페이스
    /// </summary>
    public interface IProtocol
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        Result Listen(string address, int backLog);

        /// <summary>
        /// 연결 요청을 한다. 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="connected"></param>
        /// <param name="disconnected"></param>
        Result Connect(string address);

        /// <summary>
        /// 연결을 끊는다. 
        /// </summary>
        void Disconnect();

        /// <summary>
        /// 바이트를 받는다.
        /// </summary>
        void OnReceived(MemoryStream stream);

        /// <summary>
        /// 연결 결과를 통지 받는다. 
        /// </summary>
        void OnConnected(Result result, string message);

        /// <summary>
        /// 연결을 받는다. 
        /// </summary>
        /// <param name="socket"></param>
        void OnAccepted(object socket);

        /// <summary>
        /// 연결 종료 결과를 통지 받는다.
        /// </summary>
        void OnDisconnected(Result result, string message);
    }
}
