using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnNet
{
    public class TickTimer
    {
        private int start = Environment.TickCount;

        public void Start()
        {
            start = Environment.TickCount;
        }

        public int Elapsed()
        {
            int current = Environment.TickCount;

            return (current - start);
        }
    }
}
