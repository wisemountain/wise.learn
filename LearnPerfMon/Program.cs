using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace LearnPerfMon
{
    class Program
    {
        static void Main(string[] args)
        {

            PerformanceCounter theCPUCounter =
                new PerformanceCounter("Processor", "% Processor Time", "_Total");

            for (int i = 0; i < 100; ++i)
            {
                Console.WriteLine($"Counter: {theCPUCounter.NextValue()}");
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
