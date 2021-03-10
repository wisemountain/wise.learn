		segment .data 
x 		dq 		0
scanf_format 	db 		"%ld", 0
printf_format 	db 		"fact(%ld) = %ld", 0x0a, 0

		segment .text
		global main 
		global fact 
		extern scanf 
		extern printf 

main: 
		push 	rbp 
		mov 	rbp, rsp 
		lea 	rdi, [scanf_format]
		lea 	rsi, [x]
		xor 	eax, eax 
		call 	scanf 
		mov 	rdi, [x] 
		call 	fact 
		lea		rdi, [printf_format]
		mov 	rsi, [x]
		mov 	rdx, rax
		call 	printf 
		xor 	eax, eax 
		leave 
		ret

fact: 
	n 	equ 8 
		push 	rbp
		mov 	rbp, rsp 
		sub 	rsp, 16
		cmp 	rdi, 1
		jg 		greater 
		mov 	eax, 1
		leave
		ret 

greater: 
		mov 	[rsp+n], rdi 
		dec 	rdi 
		call 	fact 
		mov 	rdi, [rsp+n]
		imul 	rax, rdi 
		leave 
		ret 

