using System;
using System.Text;
using MessagePack;

namespace LearnNet
{

    // Summary: 
    //  MsgPack은 int라도 필드 값이 작으면 필요한 길이만큼 사용. 예) 0을 int로 만들면 1바이트만 차지함.
    public class Msg 
    {
        [IgnoreMember]
        public uint Type { get; set; }

        [IgnoreMember]
        public MsgPackProtocol Protocol { get; set; }
    }

}
