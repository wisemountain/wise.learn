using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace LearnNet
{
    enum MsgInternal
    {
        Timeout,
        AcceptedForNode,
        Connected,      // 노드와 앱용
        Accepted,       // 애플리케이션 용
        Disconnected,   // 애플리케이션 용
        End
    }


    public class MsgConnected : Msg
    {
        public Result Result { get; set; }

        public MsgConnected()
        {
            Type = (uint)MsgInternal.Connected;
        }

    }

    public class MsgAcceptedNode : Msg
    {
        public Socket Socket { get; set; }

        public MsgAcceptedNode()
        {
            Type = (uint)MsgInternal.AcceptedForNode;
        }
    }

    public class MsgAccepted : Msg
    {
        public MsgAccepted()
        {
            Type = (uint)MsgInternal.Accepted;
        }
    }

    public class MsgDisconnected: Msg
    {
        public string Reason { get; set; }

        public MsgDisconnected()
        {
            Type = (uint)MsgInternal.Disconnected;
        }
    }
}
