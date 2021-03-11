		segment .bss 
a 		resb 		100 
b 		resd 		100 
		align 		8 
c 		resq 		100 

		segment .text 
		global main 
main: 
		push 	rbp
		mov 	rbp, rsp 
		sub		rsp, 16 
		leave 
		ret
