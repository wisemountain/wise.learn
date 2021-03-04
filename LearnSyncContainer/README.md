# 컨테이너 락 처리 

오브젝트(Pc, Npc)를 컨테이너로 관리하고 여러 쓰레드에서 삽입, 삭제, 조회가 
있는 상황에서 락 경쟁 (lock contention)을 최소화 하거나 락 비용을 최소화하기위한 
방법들을 살펴본다. 

## 개요 

세 가지 방향에서 최적화를 진행할 수 있다. 

- lockfree / lockless 기법의 활용 
- spinlock 최적화 
- 락 경쟁 최소화 

## concurrent_vector 실험 

concurrent_vector가 Visual C++의 Concurrency에 있어 살펴봤다. 

concurrent_vector는 erase가 없고, 원소에 대한 값 지정이 atomic 하지 않아 
락이 결국 필요하게 된다. 그래서, 문제 해결에 도움이 되지 않는다. 

단지, 락 프리 기법들이 활용 가능할 수는 있어 보인다. 

## spinlock 최적화 

spinlock 구현은 몇 가지가 있다. 
- boost.fiber의 스핀락 
- facebook folly의 RWSpinLock 

boost.fiber의 스핀락은 몇 종이 있는데 그 중에 하나가 
YieldProcessor, Sleep을 사용하여 스핀 횟수에 따라 대기에 들어간다. 
YieldProcessor는 intrinsic으로 구현되어 있는데 매우 효율적으로 
CPU를 사용하지 않고 대기할 수 있다고 한다. 
이 스핀락은 TCP 세션의 락 처리에 활용한 적이 있다. 

facebook folly의 RWSpinLock은 사용해 본 적이 없는데 
RWSpinLock의 검증된 구현으로 참고하면 좋을 듯 하다. 

## 락 경쟁 최소화 

주로 성능 이슈로 목록을 복사해서 가져와 쓸 필요가 있을 수 있다. 
이동 패킷 전송을 하게 되면 WSASend를 호출하므로 락을 잡는 시간이 길어질 수 있다. 
그래서, 복사 후 처리하는 방식으로 이해했다. 

### 락과 컨테이너 분리 

락 경쟁을 줄이려면 컨테이너와 락을 분리하는 방법을 사용할 수 있다. 
std::vector와 spinlock이나 RWSpinLock을 하나 두고 조회 시 사용하게 한다. 

### 주기적인 지연된 삭제 

insert 요청을 지연시키는 건 어려우므로 삭제만 가끔 하도록 한다. 

삭제 판단을 섹터 아이디로 하고 일정 개수 이상 삭제되거나 
일정 시간이 되면 지우는 방식이다. 

## 정리 

spinlock 최적화가 가장 중요해 보인다. 이 부분 검토를 잘 하고 측정해보면 
CPU를 많이 쓰는 문제는 해결될 수도 있어 보인다. 

응답 시간을 체감 플레이와 측정으로 분석해 볼 수 있다. 


## 다시 - context switching 비용이라면 

현재 critical section이고, context switching 비용이 높아서 발생한 문제였다면??
- YieldProcessor를 갖는 spinlock이 성능 향상을 크게 높일 수도 있다. 

rwlock이 아니고 순수 mutex 였다면 
- readwrite spinlock 또는 shared_mutex가 크게 도움이 될 수 있다. 

