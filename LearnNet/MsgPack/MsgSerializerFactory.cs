using System;
using System.Collections.Generic;

namespace LearnNet
{

    public class MsgSerializerFactory
    {
        private static MsgSerializerFactory instance;

        public static MsgSerializerFactory Instance()
        {
            if ( instance == null )
            {
                instance = new MsgSerializerFactory();
            }

            return instance;
        }

        private Dictionary<uint, MsgSerializer> serializers = new Dictionary<uint, MsgSerializer>();

        private MsgSerializerFactory()
        {
        }

        public void Set(uint msgType, Type type)
        {
            serializers[msgType] = new MsgSerializer(type, msgType);
        }

        public MsgSerializer Get(uint msgType )
        {
            if ( serializers.ContainsKey(msgType))
            {
                return serializers[msgType];
            }

            return null;
        }
    }

}
