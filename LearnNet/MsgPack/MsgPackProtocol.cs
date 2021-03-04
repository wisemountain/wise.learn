using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace LearnNet
{
    public class MsgPackProtocol : ProtocolTcp
    {
        private const int msgLengthFieldLen = sizeof(UInt32);
        private const int msgTypeFieldLen = sizeof(UInt32);
        private const int headerLen = msgLengthFieldLen + msgTypeFieldLen; 
        private MemoryStream recvStream = new MemoryStream();
        private int recvLen = 0;
        private MsgPackNode listener;
        private int messageCount = 0;
        private Guid guid;
        private Publisher pub = new Publisher();

        public int MessageCount { get { return messageCount; } }

        public Guid ProtocolId { get { return guid; } }

        public MsgPackProtocol(MsgPackNode listener)
        {
            this.listener = listener;
            guid = Guid.NewGuid();
        }

        public MsgPackProtocol(MsgPackNode listener, Socket socket)
            : base(socket)
        {
            this.listener = listener;
            guid = Guid.NewGuid();
        }

        /// <summary>
        /// 소켓을 갖고 생성했을 경우 내부 함수
        /// </summary>
        public void BeginRecvInternalFromNode()
        {
            Session.BeginRecvInternal();
        }

        public Result Send(Msg m)
        {
            var serializer = MsgSerializerFactory.Instance().Get(m.Type);

            if (serializer == null)
            {
                return Result.Fail_MsgTypeNotFound;
            }

            MemoryStream sendStream = new MemoryStream();
            serializer.Pack(sendStream, m);

            return Session.Send(sendStream.GetBuffer(), 0, (int)sendStream.Position);
        }

        public Publisher.Result Subscribe(object o, uint msgType, Action<Msg> action)
        {
            return pub.Subscribe(o, msgType, action);
        }

        public override void OnReceived(MemoryStream stream)
        {
            OnReceived(stream.GetBuffer(), 0, (int)stream.Position);
        }


        public int Post(Msg m)
        {
            return pub.Post(m);
        }

        public void Unsubscribe(object o, uint msgType)
        {
            pub.Unsubscribe(o, msgType);
        }

        public void Unsubscribe(object o)
        {
            pub.Unsubscribe(o);
        }

        public override void OnConnected(Result result, string message)
        {
            var m = new MsgConnected { Result = result, Protocol = this };
            listener.Notify(m);
        }

        public override void OnAccepted(object socket)
        {
            var m = new MsgAcceptedNode { Protocol = this, Socket = (Socket)socket };
            listener.Notify(m);
        }

        public override void OnDisconnected(Result result, string reason)
        {
            var m = new MsgDisconnected { Protocol = this, Reason = reason };
            listener.Notify(m);
        }

        public void OnReceived(byte[] rbuf, int offset, int count)
        {
            int currentRecvLen = count;

            recvStream.Write(rbuf, offset, currentRecvLen);
            recvLen += currentRecvLen;

            Contract.Assert(recvLen == recvStream.Position);

            int initialRecvLen = recvLen;
            int readOffset = 0;
            int loopCount = 0;

            // rewind to read
            recvStream.Position = 0;

            while (recvLen >= msgLengthFieldLen)
            {
                uint msgLen = 0;
                MsgSerializer.ReadUInt32(recvStream, out msgLen);

                if ( msgLen == 0 )
                {
                    throw new InvalidOperationException(
                        $"MsgLength zero! BufferLen:{recvStream.Length}, " +
                        $"CurrentRecvLen:{currentRecvLen}, RecvLen:{currentRecvLen}, " + 
                        $"LoopCount:{loopCount}"
                        );
                }

                if (recvLen < msgLen)
                {
                    break;
                }

                uint msgType = 0;
                MsgSerializer.ReadUInt32(recvStream, out msgType);

                var serializer = MsgSerializerFactory.Instance().Get(msgType);

                if (serializer == null)
                {
                }
                else
                {
                    try
                    {
                        var m = (Msg)serializer.Unpack(recvStream);
                        m.Protocol = this;
                        listener.Notify(m);

                        ++messageCount;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }

                readOffset += (int)msgLen;
                recvLen -= (int)msgLen; 
            }

            if ( readOffset > 0 )
            {
                // 버퍼를 앞으로 옮김. MemoryStream의 GetBuffer()는 내부 버퍼를 돌려줌. 
                var buf = recvStream.GetBuffer();
                int tailLen = initialRecvLen - (int)readOffset;

                if ( tailLen > 0 )
                {
                    Buffer.BlockCopy(buf, readOffset, buf, 0, tailLen);
                }
            }

            recvStream.Position = recvLen; // 현재 받은 위치 끝으로 누적하기위해 이동
        }


    }
}
