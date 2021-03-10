		segment .data 
switch: dq 		_start.case0
		dq 		_start.case1
		dq 		_start.case2

i: 		dq 		0

		segment .text
		global _start
_start:
		mov 	rax, [i]
		jmp 	[switch + rax*8]						; switch ( i )

.case0: 
		mov 	rbx, 100							; here if i == 0
		jmp 	.end

.case1: 
		mov 	rbx, 101
		jmp 	.end

.case2: 
		mov 	rbx, 102
		jmp		.end

.end
		mov 	rax, 60
		mov 	rdi, rbx
		syscall
