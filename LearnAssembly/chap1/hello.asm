	segment .text
	global _start 

_start: 
	mov eax, 60
	mov ebx, 0
	syscall
