# lock 

## 목표

recursive upgradable and downgradable shared mutex 를 만든다

## 배경 

proactor를 사용하는 asio 기반의 서버 처리 구조는 R2M에서 그 경쟁력이 입증되었다. 

post를 통한 타이머와 이벤트 콜백 처리와 함께 여러 코어를 사용하는 단일 장비에서 
분산 처리를 배제하고 처리하는 구조로 처리 코드가 단순하면서 동시성을 손쉽게 높일 
수 있다는 장점이 가장 크다. 

단지, shared state multithreading 구조이기 때문에 thread-safe 하게 만들고 
맵 내 엔티티 관리와 같이 중요한 처리 부분의 동시성을 올리면서 동시에 
thread-safe 하게 만드는 것이 함께 중요하고 어렵다. 

R2M 코드에서는 read에 대해 락을 걸지 않고 처리하는 경우가 많은데 이는 캐시를 많이 
쓰는 CPU 구조에서 잠재적인 문제들이 다수 발생하게 하는 원인이 될 뿐만 아니라 
프로그래밍 할 때 `확신 없이`, `검증 불가능한 상태`로 코딩하게 만든다. 

[1] read도 thread-safe 해야 한다. 

읽기에 락을 걸면 여러 쓰레드 간에 락 경쟁 (lock contention)이 높아질 가능성이 생기므로 
read / write 락을 쓰거나 spinlock을 사용해야 성능 문제가 발생하지 않도록 할 수 있다. 
spinlock은 CPU 사용량을 전체적으로 많이 올리 수 있기 때문에 전체에 적용하기는 어렵다. 

[2] reader / writer 락이 필요하다. 

read / write 락의 표준 구현은 shard_mutex이며 windows에서는 Slim Reader Writer Lock으로 
구현되어 있고 recursive_mutex, mutex 모두 이를 사용하고 있다. 

[3] std::shared_mutex가 적절한 reader / writer 락이다. 

한 클래스의 모든 public 함수들이 이제 적절한 락을 사용해야 하므로 한 멤버함수 내에서 
다른 멤버 함수를 호출할 때 락을 여러 번 잡게 되므로 recursive해야 한다. 
std::shared_mutex의 기본 구현은 recursive 하지 않으므로 그렇게 만들어야 한다. 

[4] recurisve 한 std::shared_mutex가 필요하다. 

또한 같은 오브젝트의 락에 대해 읽기 중에 쓰기 함수 호출, 쓰기 중에 읽기 함수 호출이 
있을 수 있고 생각보다 많으므로 reader --> writer, writer --> reader 간의 락 전환이 
중간에 필요하다. 앞의 것을 upgrade, 뒤의 것을 downgrade라고 하자. 

[5] upgrade, downgrade가 필요한 std::shared_mutex가 필요하다. 

이들 정보를 유지하는 효율적인 방법은 TLS (thread local storage)를 사용하는 것이다. 
유지해야 할 정보를 매우 작게 하고 한 메모리 위치에 둬서 쓰레드 캐시에 모두 
정보가 로딩되도록 하여 빠르게 처리 가능하게 하면서 [1]~[5]를 만족하는 
구현을 찾을 수 있다. 


## 설계와  구현 

### locked / called 

- locked : 
	- when the lock is in lock state either in shared or unique 

- called : 
	- lock is acquired by explicitly calling lock or lock_shared 
	- this is not changed after acquring the lock 
	- used to lock again when exit a lock





