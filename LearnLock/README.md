# lock 

## ��ǥ

recursive upgradable and downgradable shared mutex �� �����

## ��� 

proactor�� ����ϴ� asio ����� ���� ó�� ������ �ٸ� ���ӿ��� �� ������� �����Ǿ���. 

post�� ���� Ÿ�̸ӿ� �̺�Ʈ �ݹ� ó���� �Բ� ���� �ھ ����ϴ� ���� ��񿡼� 
�л� ó���� �����ϰ� ó���ϴ� ������ ó�� �ڵ尡 �ܼ��ϸ鼭 ���ü��� �ս��� ���� 
�� �ִٴ� ������ ���� ũ��. 

����, shared state multithreading �����̱� ������ thread-safe �ϰ� ����� 
�� �� ��ƼƼ ������ ���� �߿��� ó�� �κ��� ���ü��� �ø��鼭 ���ÿ� 
thread-safe �ϰ� ����� ���� �Բ� �߿��ϰ� ��ƴ�. 

���ӵ鿡�� read�� ���� ���� ���� �ʰ� ó���ϴ� ��찡 ������ �̴� ĳ�ø� ���� 
���� CPU �������� �������� �������� �ټ� �߻��ϰ� �ϴ� ������ �� �Ӹ� �ƴ϶� 
���α׷��� �� �� `Ȯ�� ����`, `���� �Ұ����� ����`�� �ڵ��ϰ� �����. 

[1] read�� thread-safe �ؾ� �Ѵ�. 

�б⿡ ���� �ɸ� ���� ������ ���� �� ���� (lock contention)�� ������ ���ɼ��� ����Ƿ� 
read / write ���� ���ų� spinlock�� ����ؾ� ���� ������ �߻����� �ʵ��� �� �� �ִ�. 
spinlock�� CPU ��뷮�� ��ü������ ���� �ø� �� �ֱ� ������ ��ü�� �����ϱ�� ��ƴ�. 

[2] reader / writer ���� �ʿ��ϴ�. 

read / write ���� ǥ�� ������ shard_mutex�̸� windows������ Slim Reader Writer Lock���� 
�����Ǿ� �ְ� recursive_mutex, mutex ��� �̸� ����ϰ� �ִ�. 

[3] std::shared_mutex�� ������ reader / writer ���̴�. 

�� Ŭ������ ��� public �Լ����� ���� ������ ���� ����ؾ� �ϹǷ� �� ����Լ� ������ 
�ٸ� ��� �Լ��� ȣ���� �� ���� ���� �� ��� �ǹǷ� recursive�ؾ� �Ѵ�. 
std::shared_mutex�� �⺻ ������ recursive ���� �����Ƿ� �׷��� ������ �Ѵ�. 

[4] recurisve �� std::shared_mutex�� �ʿ��ϴ�. 

���� ���� ������Ʈ�� ���� ���� �б� �߿� ���� �Լ� ȣ��, ���� �߿� �б� �Լ� ȣ���� 
���� �� �ְ� �������� �����Ƿ� reader --> writer, writer --> reader ���� �� ��ȯ�� 
�߰��� �ʿ��ϴ�. ���� ���� upgrade, ���� ���� downgrade��� ����. 

[5] upgrade, downgrade�� �ʿ��� std::shared_mutex�� �ʿ��ϴ�. 

�̵� ������ �����ϴ� ȿ������ ����� TLS (thread local storage)�� ����ϴ� ���̴�. 
�����ؾ� �� ������ �ſ� �۰� �ϰ� �� �޸� ��ġ�� �ּ� ������ ĳ�ÿ� ��� 
������ �ε��ǵ��� �Ͽ� ������ ó�� �����ϰ� �ϸ鼭 [1]~[5]�� �����ϴ� 
������ ã�� �� �ִ�. 


## �����  ���� 

### locked / called 

- locked : 
	- when the lock is in lock state either in shared or unique 

- called : 
	- lock is acquired by explicitly calling lock or lock_shared 
	- this is not changed after acquring the lock 
	- used to lock again when exit a lock

### unlock / lock ����� �� ��ȯ ���� 

ª���� �ٸ� �����尡 ���� ��� ������ ������ ����� ���� �ٽ� �о�� �Ѵ�. 

���ɶ��� �ϳ� �ΰ� ���� ���� ���¿��� upgrade / downgrade�� �����ϸ� ���? 


## ���� 

### ���� 

- std::mutex �� 
	- 2 readers and 2 writers
	- 4 core ��� 
	- 1õ�� ����
	- lockable: 
		-  1456ms, 1172ms, 1160ms 
	- mutex: 
		- 1167ms, 1199ms, 1214ms 

������ �ʰ� reader ���ü��� �ö󰡹Ƿ� ������. 

### ��Ȯ�� 

�� ������ upgrade / downgrade�� �����Ͽ� ġ���� ��Ȳ���� �� �����Ѵ�. 

������ ������ LTL(Linear temporal logic)�� transition system�� �����Ͽ� 
�����ϵ��� �Ѵ�. 


### upgrade ����� 

https://oroboro.com/upgradable-read-write-locks/

Upgradable Write Locks and Deadlock
Using read/write locks with upgrading is tricky. You only want to upgrade a read/write 
lock in cases where you are sure only one active reader will eventually want to write lock.

Consider the following situation:

Thread 1 acquires a read lock
Thread 2 acquires a read lock
Thread 1 tries to acquire a write lock, and is blocked on thread 2��s read lock
Thread 2 tries to acquire a write lock, and is blocked on thread 1��s read lock.

���� ����� ���� ���� ��� �ִ� ���¿��� ���׷��̵带 �� ��� ������� �߻��Ѵ�. 

lockable / lock_thread_tracer�� �������� �Ұ����ϰ� unlock / unlock_shared�� ����ϴ�
�� ���׷��̵� / �ٿ�׷��̵带 ����߰� �̿� ���� ������ ������� �߻����� �ʴ´�. 

xlock ���� ������� �߻��ϴ� �κ��� ������ �����Ѵ�. 

### xlock ����� 

������� ���ϰų� �߻��� ��� �ذ��� �� �־�� �Ѵ�. ����� Ž�� �� ó���� �ָ��ϹǷ� 
�� ���� �ִ��� ������� ���� �� �ִ� ���������� �����ϰ� �ؾ� �Ѵ�. 

�Ʒ� ��뿡�� �� �����Ѵ�. 

## ���� 

������ �Բ� ������ �����Ѵ�. 


### ����� 



### ����� detection 



## ��� 

### ���� 

- container�� object

�ٸ� ������Ʈ�� ��� �ڷ� �����̴�. Map �Ǵ� Level�� ���ο� ���ԵǴ� Sector�� �����̳ʷ� 
�� ���� �ִ�. Sector�� ���� Entity�� ��� container�� �� �� �ִ�. 

���� ������ UserManager, GuildManager�� ���� Manager�鵵 �����̳ʷ� �� �� �ִ�. �ٸ� �������� 
�̵��� ó����� ������ ���� �ִ�. ó����� ó�� ���� �����ϴ� Ŭ�����̴�. 

object�� �̵� �����̳ʿ� ���� ����̴�. GuildManager�� Guild�� �����Ѵ�. 
UserManager�� User�� �����Ѵ�. �̵� object�� ���� ���� �����ϴ� �뵵�� ����Ѵ�. 

- entity 

entity�� ���������� slock�� xlock���� thread-safe�ϰ� �Ѵ�. 

- handler 

�ڵ鷯�� ��Ŷ�̳� Ÿ�̸� ȣ���� ó���ϴ� �Լ��� ���� ���� ó���� ��� ���������̴�. 
����, handler�鿡�� ��� �� �帧�� ������ �� ó�� ������ �ȴ�. 

- view

���� ������ Ʈ������� ó���ϴ� �ý����� �ƴ� ������ ���� ���¿� ���� ���� ���·� 
�Űܰ��� �ý����̴�. ���� ������ ���鿡 ���� ó���Ͽ� ���� ���·� �����Ѵٴ� �������� 
`������ ����`�� ���� ����Ѵ�. 

view�� ���ӿ��� ���� ���� read uncommitted isolation ���ذ� ���� ������ Ʈ����� ó���� 
�ƴϴ��� ���� ����� ���鿡�� ���� ���� ó���Ǵ��� ���� �ϰ���(consistency)�� ����
�� �� �ִٴ� ������ ����� �� �ִ� �����̴�. 


### ���̵� 

[1] container�� ���������� thread-safe�ϰ� �Ѵ�. 

[2] object�� ���������� thread-safe�ϰ� �Ѵ�. 

[3] entity�� ���������� xlock/slock���� thread-safe�ϰ� �Ѵ�. 

[4] handler�� ��� ������Ʈ�� ���� view���� �� ������Ʈ ó���� �Ѵ�. 
    ��, �Լ� ȣ��� ���� ������ �� ������Ʈ �Լ����� ȣ���Ѵ�. 

[5] handler�� ��� ������Ʈ�� ���� slock�� �� �����ϸ� slock ȣ���� ���� �� �ְ� 
	���ÿ� xlock���κ��� ��ȣ�� �� �ִ�. 

[6] �ٸ� entity�� ���� xlock�� ���� �� slock���� downgrade�ϰ� ȣ���ϴ��� xlock�� Ǯ�� ȣ���ϸ� 
	������� ���� �� �ִ�. 

[7] concurrent ������ ���� �� ���� ������ ���ٸ� xlock, slock�� ����Ѵ�. 

