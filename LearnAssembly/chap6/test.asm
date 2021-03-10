		segment .data 
a		dq		151
b 		dq 		310
sum 	dq 		0

		segment .text
		global _start
_start:
		push 	rbp
		mov		rbp, rsp 
		sub 	rsp, 16 
		mov 	rax, 9
		add		[a], rax
		mov 	rax, [b] 
		add		rax, 10
		add 	rax, [a]
		mov 	[sum], rax
		mov 	rax, 0
	
		mov 	rax, 60
		mov 	rdi, [sum]
		syscall
