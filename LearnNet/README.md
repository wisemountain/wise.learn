# 통신 

MsgPack 기반. 약간 더 일반화 해서 Tcp 상에서 임의의 프로토콜 실행이 가능하도록 정리한다. 



# 설계

- IProtocol 
  - 최상위 인터페이스 
- ProtocolTcp
  - SessionTcp 를 내부에서 사용 
  - Tcp 통신은 여기서 다 처리 
- MsgPackProtocol 
  - MsgPack 통신이 거의 다 됨 



# 구현



## MsgPack 프로토콜 

실제 내용이 있어야 테스트가 가능하므로 MsgPack을 예시로 작성. 예전에 작성했던 Warp 코드 참조. 

- Serialize / Deserialize 
  - Serialize(Type type, Stream stream, object obj, ...);
  - object Deserialize(Type type, Stream stream, ...);



- NetStream의 Read가 이상해 보임 
  - MemoryStream으로 변경



- Msg와 메세지 타잎간 연결이 느슨하다. 
  - 쉽게 지정할 방법은 나중에 더 본다. 



## MsgPackProtocol 

- 단위 테스트는 경험만큼 중요하다. 
  - 서버 단위 테스트를 확보해서 참 좋다. 봇 전문가. 



## MsgPackListener 

- 프로토콜들 관리 
- 앱과 연결 
- Client / Server의 기반 클래스 



## MsgPackServer 

- Listen 
- 메세지 전달은 어떻게? 
  - Subscription을 어떻게?





# 디스패칭과 실행



## 실행 모델 

서버 실행 모델에 의존하는 부분이 있다. 네트워크 쓰레드에서 직접 앱 기능을 실행하는 걸 선호하는 경우도 있고 비동기로 앱 쓰레드를 따로 두는 경우도 있다. 두 가지 모델을 모두 지원하는 방법이 필요하다. 



## 프로토콜에 연결 

타잎으로만 디스패칭을 하면 프로토콜 상태에 따른 처리가 어려워질 수 있다. 따라서, Subscription을 프로토콜에 직접 할 수 있게 하는 게 바로 통지 받을 수 있어 좋다. 



## 언제 어디서 포스팅 

프로토콜 (이전의 세션)에 연결. 필요한 모델에 따라 구현. 현재는 Run()에서 호출하는 방식. 큐는 갖고 있다. 정책으로 분리해서 나누어 구현할 수도 있지만 그렇게 할 필요는 없을 듯 싶다. 

채널을 두고 채널로 일단 보내게 하는 방식으로도 가능하다. 하나를 취하면 단순해지나 유연하지 않다. 



# Accept 흐름 구현 

Accept를 C#에서 비동기로 구현한 적이 없어 이번에 구현한다. 받는 흐름만 제대로 만들어지면 된다. 

생각보다 오래 걸렸다. 역시 통신은 민감한 부분이 있다. 

- Posting과 Subscription 간의 조절 
  - 포스팅 중에 Subscribe 호출이 가능 
- 콜백들 조절 



# MessagePack Serialization 처리와 통신 



```c#
// MessagePack Friendly
[assembly: InternalsVisibleTo("MessagePack")]
[assembly: InternalsVisibleTo("MessagePack.Resolvers.DynamicObjectResolver")]
[assembly: InternalsVisibleTo("MessagePack.Resolvers.DynamicUnionResolver")]
```

위를 추가해야 제대로 된다. 

송신 되고 에코 받아서 수신 처리하는 부분에서 막힘 

```c#
var t = DynamicObjectResolver.Instance.GetFormatter<MsgEcho>();
// MessagePack.Formatters.TestObjectFormatter
Console.WriteLine(t.GetType().FullName);
// MessagePack.Resolvers.DynamicObjectResolver, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
Console.WriteLine(t.GetType().Assembly.FullName);
```

Resolver 확인. 



# 정리 

- MessagePack은 전에 MsgPack 라이브러리보다 많이 개선된 걸로 보인다. 
- C# 통신도 매우 빠르다. 
- Acceptor 추가가 좀 걸렸지만 이틀만에 작업을 끝냈다. 
- Session과 프로토콜 풀링을 해야 할 지 생각을 좀 해봐야 한다. 
  - 안정적인 게 더 나을 수도 있는데 성능 영향이 크다면 고려한다. 
  - C#에서 그 정도는 Generational GC로 인해 괜찮을 듯 싶다. 
- 













