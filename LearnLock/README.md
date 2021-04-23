# lock 

## ��ǥ

recursive upgradable and downgradable shared mutex �� �����

## ��� 

proactor�� ����ϴ� asio ����� ���� ó�� ������ R2M���� �� ������� �����Ǿ���. 

post�� ���� Ÿ�̸ӿ� �̺�Ʈ �ݹ� ó���� �Բ� ���� �ھ ����ϴ� ���� ��񿡼� 
�л� ó���� �����ϰ� ó���ϴ� ������ ó�� �ڵ尡 �ܼ��ϸ鼭 ���ü��� �ս��� ���� 
�� �ִٴ� ������ ���� ũ��. 

����, shared state multithreading �����̱� ������ thread-safe �ϰ� ����� 
�� �� ��ƼƼ ������ ���� �߿��� ó�� �κ��� ���ü��� �ø��鼭 ���ÿ� 
thread-safe �ϰ� ����� ���� �Բ� �߿��ϰ� ��ƴ�. 

R2M �ڵ忡���� read�� ���� ���� ���� �ʰ� ó���ϴ� ��찡 ������ �̴� ĳ�ø� ���� 
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





