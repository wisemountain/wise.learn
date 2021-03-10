		segment .data 

data 	db 		"hello world", 0
n 		dq 		0
needle 	db 		'w'

		segment .text 
		global _start

		; find needle in data 
		; - a simple search of a character in a null terminated string

_start:
		push 	rbp 
		mov 	rbp, rsp 
		sub 	rsp, 16 

		; end of initialization 

		mov 	bl, [needle] 		; rbx = needle
		xor 	ecx, ecx			; rcx = 0
		mov 	al, [data+rcx] 		; rax = data[rcx] 
		cmp 	al, 0 				; rax compare 0 
		jz 		end_while 			; if al == 0 goto end_while

while: 
		cmp 	al, bl 				; data[rcx] compare 'w'
		je 		found 				; if ( al == bl ) goto found
		inc 	rcx 				; ++rcx 
		mov 	al, [data+rcx] 		; rax = data[rcx]
		cmp 	al, 0 				; 
		jnz 	while				; if al == 0 then goto while

end_while: 
		mov 	rcx, -1 			; rcx = -1
found: 
		mov 	[n], rcx			; n = rcx 
		xor 	eax, eax 			; eax = 0

exit: 
		mov 	rax, 60 
		mov 	rdi, [n]
		syscall

		
