# performance monitor

가끔 써 봤다. 리소스 모니터나 작업관리자가 편하니 주로 그걸로 해결했다. 
또 긴 시간 모니터링이 필요하면 SNMP 기반의 관리 도구를 사용했다. 

이 중간에 실시간성을 유지하면서 로그도 남길 수 있는 도구가 필요하고 
API로 프로그래밍 기능도 제공하는 기능이 있어 자세히 살펴보려 한다. 

csv로 로깅을 남기면 파이썬으로 분석하고 리포트를 제공하는 흐름이 
매우 좋다. 이 흐름을 만들기위해 REST로 에이전트에 요청하여 
현재 시점의 카운터 정보를 받을 수 있게 한다. 

여기서 REST까지는 C#과 파이썬으로 구현하고 테스트해 두었다. 

## 개념과 활용 

작업 관리자에서 볼 수 있는 항목들을 깔끔하게 비슷하게 볼 수 있어야 한다. 
그 다음에 세부항목을 추가해나간다. 

perfmon.exe를 실행하면 처음에 뜨는 항목들이 다음과 같다. 

- Memory 
    - % Committed Bytes in Use 
    - Available MBytes 
    - Cache Faults/Sec

- Network Interface
  - Bytes Total/sec

- Physical Disk
  - % Idle Time 
  - Avg. Disk Queue Length

- Processor Information 
  - % Interrupt Time 
  - % Processor Time 

시스템의 핵심을 보여주는 항목들이다. 

## API 

https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.performancecounter?view=netframework-4.8

샘플이 있어 여기에 기초하여 진행한다. perfmon에서 보는 문자열과 다를 수 있다. 

```c#
  PerformanceCounter theCPUCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

  for (int i = 0; i < 100; ++i)
  {
    Console.WriteLine($"Counter: {theCPUCounter.NextValue()}");
    System.Threading.Thread.Sleep(1000);
  }
```            
엄청 단순하고 간결하다. 좋다. 

