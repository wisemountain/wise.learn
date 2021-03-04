using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace LearnTopShelf
{
    class AgentService 
    {
        public void Start()
        {
            Console.WriteLine("Starting");
        }

        public void Stop()
        {
            Console.WriteLine("Stopped");
        }
    }

    class AgentService2 : ServiceControl
    {
        public AgentService2()
        {
        }

        public bool Start(HostControl hc)
        {
            Console.WriteLine("Start AgentService2");
            return true;
        }

        public bool Stop(HostControl hc)
        {
            Console.WriteLine("Stop AgentService2");
            return true;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // SetupSimplest();
            Setup2();
        }

        static void SetupSimplest()
        {
            var rc = HostFactory.Run(x =>
            {
                x.Service<AgentService>(s =>
                {
                    s.ConstructUsing(name => new AgentService());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();

                x.SetDescription("Sample Topshelf Host");
                x.SetDisplayName("Stuff");
                x.SetServiceName("Stuff");

                x.StartAutomatically();

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(0);
                    r.OnCrashOnly();
                    r.SetResetPeriod(1);
                });
            });

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());
            Environment.ExitCode = exitCode;
        }

        static void Setup2()
        {
            var host = HostFactory.New(x =>
            {
                x.Service<AgentService2>(sc =>
                {
                    sc.ConstructUsing((hostControl) => new AgentService2());
                    sc.WhenStarted((s, hostControl) => s.Start(hostControl));
                    sc.WhenStopped((s, hostControl) => s.Stop(hostControl));
                });

                x.RunAsLocalSystem();

                x.SetDescription("Sample Topshelf Host");
                x.SetDisplayName("Stuff");
                x.SetServiceName("Stuff");

                x.StartAutomatically();

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(0);
                    r.OnCrashOnly();
                    r.SetResetPeriod(1);
                });
            });

            host.Run();
        }
    }
}
