# lock 

- make shared mutex reentrant (recursive mutex)
- lock upgrade / downgrade 
- keep and solo mode 


## �����  ���� 

### enter_xlock_keep(lockable* lock)
  
- xlock_keep�� �̹� ��� �ִ� �� Ȯ�� 
- �ƴ϶�� ��� current�� ���� 
- �ִٸ� count�� ���� ��Ŵ


�����ؾ� �ϴµ� ���δ�. ���� �� �ؼ� ����� ������ ã�´�. ������ ������. 


### exit_xlock_keep(lockable* lock)


### enter_slock_keep(lockable* lock)


### exit_slock_keep(lockable* lock)


### enter_xlock_solo(lockable* lock)


### exit_xlock_solo(lockable* lock)


### enter_slock_solo(lockable* lock)


### exit_slock_solo(lockable* lock)


### ���� 1

XK(1)
		[0] { type_: xlock_keep, locked_ :true, prev_: -1, called_ : true}
		current = 0

	XS(2)
		[0] { type_: xlock_keep, locked_ : false , prev_: -1, called_ : true}
		[1] { type : xlock_solo, locked_ : true, prev: -1, called_ : true}
		current = 1

	~XS(2)
		[0] { type_: xlock_keep, locked_ : true, prev_: -1, called_ : true, }
		[1] { type : xlock_solo, locked_ : true, prev: -1, called_ : false, invalid_ : true}
		current = 0
		
~XK(1)
		[0] { type_: xlock_keep, locked_ : true, prev_: -1, called_ : true, invalid_ : true}
		current = -1
	

�׽�Ʈ�� ���� �����ϴ� ���� ���� �� �ϴ�. ���� �����忡�� ���� �� �Ŀ� ������ ��Ȯ�ϰ� �����ϴ� �� Ȯ��. 





