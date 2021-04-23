# lock 

- make shared mutex reentrant (recursive mutex)
- lock upgrade / downgrade 
- keep and solo mode 


## 설계와  구현 

### enter_xlock_keep(lockable* lock)
  
- xlock_keep로 이미 잡고 있는 지 확인 
- 아니라면 잡고 current를 지정 
- 있다면 count를 증가 시킴


간결해야 하는데 꼬인다. 여러 번 해서 깔끔한 구현을 찾는다. 방향은 괜찮다. 


### exit_xlock_keep(lockable* lock)


### enter_slock_keep(lockable* lock)


### exit_slock_keep(lockable* lock)


### enter_xlock_solo(lockable* lock)


### exit_xlock_solo(lockable* lock)


### enter_slock_solo(lockable* lock)


### exit_slock_solo(lockable* lock)


### 검증 1

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
	

테스트를 통해 검증하는 것이 좋을 듯 하다. 단일 쓰레드에서 먼저 한 후에 락들이 정확하게 동작하는 지 확인. 





