using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace LearnCsharp
{
    class Program
    {
        static void Main(string[] args)
        {
            MemoryStream stream = new MemoryStream();

            var s = "01234567890123";

            byte[] payload = Encoding.UTF8.GetBytes(s);

            stream.Write(payload, 0, payload.Length);

            byte[] buf = stream.GetBuffer();

            string str1 = Encoding.UTF8.GetString(buf);

            Buffer.BlockCopy(buf, 5, buf, 0, 9);

            byte[] buf2 = stream.GetBuffer();
            string str2 = Encoding.UTF8.GetString(buf2);
        }
    }
}
