using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MessagePack;
using LearnNet;

namespace LearnNetTest
{
    [MessagePackObject]
    public class MyClass : Msg
    {
        // Key is serialization index, it is important for versioning.
        [Key(0)]
        public int Age { get; set; }

        [Key(1)]
        public string FirstName { get; set; }

        [Key(2)]
        public string LastName { get; set; }

        // public members and does not serialize target, mark IgnoreMemberttribute
        [IgnoreMember]
        public string FullName { get { return FirstName + LastName; } }
    }

    [TestClass]
    public class TestMsgPack
    {
        [TestMethod]
        public void TestBasicUsage()
        {
            var mc = new MyClass
            {
                Age = 99,
                FirstName = "hoge",
                LastName = "huga",
            };

            var type = typeof(MyClass);
            var stream = new MemoryStream();

            // call Serialize/Deserialize, that's all.
            byte[] bytes = MessagePackSerializer.Serialize(type, mc);
            stream.Write(bytes, 0, bytes.Length);

            stream.Position = 0;
            MyClass mc2 = (MyClass)MessagePackSerializer.Deserialize(type, stream);

            // ConvertToJson 으로 디버깅 가능

            Assert.IsTrue(mc2.Age == mc.Age);
            Assert.IsTrue(mc2.FirstName == mc.FirstName);
        }

        [TestMethod]
        public void TestMsgSerializer()
        {
            var mc = new MyClass
            {
                Age = 99,
                FirstName = "hoge",
                LastName = "huga",
            };

            var stream = new MemoryStream();
            var serializer = new MsgSerializer(typeof(MyClass), 1); 
            serializer.Pack(stream, mc);

            stream.Position = 0;

            uint msgLen = 0;
            MsgSerializer.ReadUInt32(stream, out msgLen);

            uint msgType = 0;
            MsgSerializer.ReadUInt32(stream, out msgType);
            Assert.IsTrue(msgType == 1);

            var mc2 = (MyClass)serializer.Unpack(stream);

            Assert.IsTrue(mc2.LastName == mc.LastName);
        }

        [TestMethod]
        public void TestMsgSerializerFactory()
        {
            var mc = new MyClass
            {
                Age = 99,
                FirstName = "hoge",
                LastName = "huga",
            };

            MsgSerializerFactory.Instance().Set(1, typeof(MyClass));

            var stream = new MemoryStream();
            var serializer = MsgSerializerFactory.Instance().Get(1);
            serializer.Pack(stream, mc);

            uint msgPackLen = (uint)stream.Position;

            stream.Position = 0;

            uint msgLen = 0;
            MsgSerializer.ReadUInt32(stream, out msgLen);
            Assert.IsTrue(msgPackLen == msgLen);

            uint msgType = 0;
            MsgSerializer.ReadUInt32(stream, out msgType);
            Assert.IsTrue(msgType == 1);

            var mc2 = (MyClass)serializer.Unpack(stream);

            Assert.IsTrue(mc2.LastName == mc.LastName);
        }

        [TestMethod]
        public void TestMemoryStreamBlockCopy()
        {
            string test = "Hello String";

            var stream = new MemoryStream();
            var bytes = System.Text.Encoding.UTF8.GetBytes(test);

            stream.Write(bytes, 0, bytes.Length);
            var buf = stream.GetBuffer();

            Buffer.BlockCopy(buf, 6, buf, 0, bytes.Length - 6);

            var result = System.Text.Encoding.UTF8.GetString(buf);
            var buf2 = stream.GetBuffer();

            for ( int i=0; i<buf2.Length; ++i)
            {
                Assert.IsTrue(buf[i] == buf2[i]);
            }
        }

        [TestMethod]
        public void TestMsgPackProtocolRecv_1()
        {
            // 수신을 확인한다. 

            MsgSerializerFactory.Instance().Set(1, typeof(MyClass));

            var l = new MsgPackNode();
            var p = new MsgPackProtocol(l);

            var mc = new MyClass
            {
                Age = 99,
                FirstName = "hoge",
                LastName = "huga",
            };

            var stream = new MemoryStream();
            var serializer = MsgSerializerFactory.Instance().Get(1);

            serializer.Pack(stream, mc);

            p.OnReceived(stream);

            Assert.IsTrue(p.MessageCount == 1);

            var mc2 = (MyClass)l.Next();
            Assert.IsTrue(mc2 != null);
            Assert.IsTrue(mc2.FirstName == mc.FirstName);
            Assert.IsTrue(mc2.Protocol == p);
        }

        [TestMethod]
        public void TestMsgPackProtocolRecv_N()
        {
            // 수신을 확인한다. 

            MsgSerializerFactory.Instance().Set(1, typeof(MyClass));

            var l = new MsgPackNode();
            var p = new MsgPackProtocol(l);

            var mc = new MyClass
            {
                Age = 99,
                FirstName = "hoge",
                LastName = "huga",
            };

            var stream = new MemoryStream();
            var serializer = MsgSerializerFactory.Instance().Get(1);

            const int testCount = 11;

            for (int i = 0; i < testCount; ++i)
            {
                mc.Age = i;

                serializer.Pack(stream, mc);
            }

            p.OnReceived(stream);

            Assert.IsTrue(p.MessageCount == testCount);

            for (int i = 0; i < testCount; ++i)
            {
                var mc2 = (MyClass)l.Next();

                Assert.IsTrue(mc2 != null);
                Assert.IsTrue(mc2.Age == i);
                Assert.IsTrue(mc2.FirstName == mc.FirstName);
                Assert.IsTrue(mc2.Protocol == p);
            }
        }

        [TestMethod]
        public void TestMsgPackProtocolRecv_Partial()
        {
            // 수신을 확인한다. 

            MsgSerializerFactory.Instance().Set(1, typeof(MyClass));

            var l = new MsgPackNode();
            var p = new MsgPackProtocol(l);

            var mc = new MyClass
            {
                Age = 99,
                FirstName = "hoge",
                LastName = "huga",
            };

            var stream = new MemoryStream();
            var serializer = MsgSerializerFactory.Instance().Get(1);

            serializer.Pack(stream, mc);

            p.OnReceived(stream.GetBuffer(), 0, 5);
            Assert.IsTrue(l.Next() == null);

            Assert.IsTrue(p.MessageCount == 0);

            p.OnReceived(stream.GetBuffer(), 5, (int)stream.Position - 5);

            Assert.IsTrue(p.MessageCount == 1);

            var mc2 = (MyClass)l.Next();
            Assert.IsTrue(mc2 != null);
            Assert.IsTrue(mc2.FirstName == mc.FirstName);
            Assert.IsTrue(mc2.Protocol == p);
        }

        [TestMethod]
        public void TestFind()
        {
            var lst = new List<MyClass>();

            lst.Add(new MyClass { Age = 3});
            lst.Add(new MyClass { Age = 9});

            var m  = lst.Find(x => x.Age == 10);
            Assert.IsTrue(m == null);
        }

        [TestMethod]
        public void TestSubscription()
        {
            var pub = new Publisher();
            var obj = new object();

            pub.Subscribe(obj, 1, (x) => { });
            pub.Subscribe(obj, 2, (x) => { });

            Assert.IsTrue(pub.GetSubscriptionCount(1) == 1);
            Assert.IsTrue(pub.GetSubscriptionCount(2) == 1);
            Assert.IsTrue(pub.GetSubscriptionCount(3) == 0);

            pub.Unsubscribe(obj, 2);
            Assert.IsTrue(pub.GetSubscriptionCount(2) == 0);

            var c1 = pub.Post(new Msg { Type = 1 });
            Assert.IsTrue(c1 == 1);

            pub.Unsubscribe(obj);

            Assert.IsTrue(pub.GetSubscriptionCount(1) == 0);
            Assert.IsTrue(pub.GetSubscriptionCount(obj) == 0);
        }
    }
}
