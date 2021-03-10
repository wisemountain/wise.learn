	segment .data
a 	dq 	175
b 	dq 	4097
	
	segment .text
	global _start
_start:
	mov 	rax, [a] 	; mov a into rax 
	add		rax, [b] 	; add b to rax 

	xor 	rax, rax 
	mov 	rax, 60
	mov 	rdi, 0
	syscall
