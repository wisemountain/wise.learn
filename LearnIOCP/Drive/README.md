# Iocp �׽�Ʈ 

����  ������ ���� �ΰ�, �۰� ū ��Ŷ�� �ְ� �޴� Iocp ��带 �����. 
�̸� ���� 1-send�� n-send ���� ���� ���̿� Ŀ�� ������ Ȯ���Ѵ�. 

�� ������ ���� �ּ�ȭ�� Iocp ��� ���̺귯���� �����. 

## ���� 

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
  * boost�� fixed allocator ���

### ���� ���� 

* std::array<Slot, 2000> sessions_
  - Slot = <TcpSession, Seq>
  - Id = <index, sequence>
* std::vector<int> free_slots_ 
* std::recursive_mutex lock_

## �׽�Ʈ 

* 2õ ������ n start echo 
  * n=10 ������ �׽�Ʈ 
* ���� �ٸ� ���� ��� 


## WSASend ����� ���� 

* __imp_WSASend
* mswsock.dll!WSPSend() 
* NtDeviceIoControlFile




