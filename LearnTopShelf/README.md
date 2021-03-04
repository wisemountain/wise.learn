# Learn TopShelf 

TopShelf는 console 앱을 서비스로 설치하고 실행 가능하게 해준다. 



## 가장 단순한 구성 

```c#
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
            });                                                  

            var exitCode = (int)Convert.ChangeType(rc, rc.GetTypeCode());  
            Environment.ExitCode = exitCode;
```

위 구조가 가장 단순하다. 더 상세한 제어를 하려면 좀 더 공부가 필요하다. 



## 서비스 구성 

```c#
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
```

sc.ConstructUsing의 람다 지정을 이해하는데 좀 걸렸다. 항상 hostControl을 아규먼트로 갖는 버전을 써야 한다. 



