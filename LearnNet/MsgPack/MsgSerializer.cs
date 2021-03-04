using System;
using System.IO;
using MessagePack;

namespace LearnNet
{
    // Summary: 
    //  Message serializer template for Message subclasses
    public class MsgSerializer 
    {
        private Type type;
        private uint msgType;

        public MsgSerializer(Type type, uint msgType)
        {
            this.type = type;
            this.msgType = msgType;
        }

        // Summary: 
        //  오브젝트 타잎을 돌려준다.
        public Type Type
        {
            get { return type; }
        }

        public uint MsgType
        {
            get { return msgType;  }
        }

        // Summary: 
        //  See IMessageSerializer::Pack
        public void Pack(Stream stream, object obj)
        {
            long startPosition = stream.Position;   // remember position

            WriteUInt32(stream, (uint)0); // Length
            WriteUInt32(stream, (uint)msgType);

            byte[] so = MessagePackSerializer.Serialize(type, obj);
            stream.Write(so, 0, so.Length);

            long newPosition = stream.Position;     // remember position
            stream.Position = startPosition;        // rewind

            uint messageLen = (uint)(newPosition - startPosition);
            WriteUInt32(stream, (uint)messageLen);
            stream.Position = newPosition;          // forward
        }

        // Summary: 
        //  See IMessageSerializer::Unpack
        public object Unpack(Stream stream)
        {
            // stream.Position은 type을 읽은 바로 다음이다. 

            long startPosition = stream.Position;   // remember position
            var obj = MessagePackSerializer.Deserialize(type, stream);
            long endPosition = stream.Position;

            return obj;
        }

        public static void WriteUInt32(Stream stream, uint value)
        {
            stream.WriteByte((byte)(value & 0xFF));
            stream.WriteByte((byte)(value >> 8 & 0xFF));
            stream.WriteByte((byte)(value >> 16 & 0xFF));
            stream.WriteByte((byte)(value >> 24 & 0xFF));
        }

        public static void ReadUInt32(Stream stream, out uint value)
        {
            int v1 = stream.ReadByte();
            int v2 = stream.ReadByte();
            int v3 = stream.ReadByte();
            int v4 = stream.ReadByte();

            value = (uint)(v4 << 24 | v3 << 16 | v2 << 8 | v1);
        }
    }

}
