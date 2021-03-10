		section .data 
msg: 	db 		"Hello World!", 0x0a, 0

		section .text 
		global main 				; gcc -o hello_world hello_world.o로 링크 
		extern printf

main: 
		push 	rbp 				; stack의 16 byte align을 맞춤 
		mov 	rbp, rsp
		lea 	rdi, [msg]
		xor		eax, eax
		call 	printf 
		xor 	eax, eax 
		pop 	rbp 
		ret


