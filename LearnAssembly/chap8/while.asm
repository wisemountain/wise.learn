		segment .data 
data 	dq 		0xfedcba9876543210 
sum 	dq 		0

		segment .text 
		global _start 
_start:
		
		push 	rbp 		; save rbp 
		mov 	rbp, rsp 	; move rsp to rbp 
		sub 	rsp, 16 	; move stack up 


		mov 	rax, [data] 
		xor 	ebx, ebx 
		xor 	ecx, ecx
		xor 	edx, edx 

while: 
		cmp 	rcx, 64		; rcx < 64 
		jnl 	end_while
		bt 		rax, 0
		setc 	bl 
		add		edx, ebx
		shr 	rax, 1
		inc 	rcx
		jmp 	while 
	
end_while: 
		mov 	[sum], rdx 
		xor 	eax, eax

exit:
		mov		rax, 60 
		mov 	rdi, [sum] 
		syscall
