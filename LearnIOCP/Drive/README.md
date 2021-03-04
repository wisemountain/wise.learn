# Iocp 테스트 

여러  연결을 서로 맺고, 작고 큰 패킷을 주고 받는 Iocp 노드를 만든다. 
이를 통해 1-send와 n-send 간의 성능 차이와 커널 동작을 확인한다. 

이 과정을 통해 최소화된 Iocp 통신 라이브러리를 만든다. 

## 구조 

* IocpNode 
  * Listen 
  * Connect
  * Send(uint8_t* payload, int len)
  * Close

* TcpSession 
  * OnConnected 
  * OnAccepted
  * OnReceive
  * OnDisconnected

* Buffer Pool 
  * boost의 fixed allocator 사용

### 세션 관리 

* std::array<Slot, 2000> sessions_
  - Slot = <TcpSession, Seq>
  - Id = <index, sequence>
* std::vector<int> free_slots_ 
* std::recursive_mutex lock_

## 테스트 

* 2천 연결의 n start echo 
  * n=10 정도로 테스트 
* 서로 다른 물리 장비 


## WSASend 어셈블리 추적 

* __imp_WSASend
* mswsock.dll!WSPSend() 
* NtDeviceIoControlFile




