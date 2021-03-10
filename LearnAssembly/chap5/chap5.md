# Chapter 5. Registers

- ax - accumulator for numeric operations
- bx - base register (array access)
- cx - count register (string operations)
- dx - data register
- si - source index
- di - destination index
- bp - base pointer (for function frames)
- sp - stack pointer

a, b, c, d에 대해 l과 h로 1 바이트 씩 접근 가능 

- eax, ebx, ecx, edx, esi, edi, ebp, esp

- rax, rbx, rcx, rdx, rsi, rdi, rbp, rsp, r8~r15 

- rflags (eflags) 

## 5.1 Moving a constant into a register


mov 	rax, 100
mov 	eax, 100

gdb:

- list (l)
- break (b)
- print (p), p/x expression 
- nexti

## 5.2 Moving values from memory into registers 

- movsx		; move sign extended
- movzx		; move zero extended

## 5.3 Moving values from a register into memory 

- mov 	[a], rax		; move content of rax to [a]

## 5.4 Moving data from one registe rto another 

- mov 	rbx, rax	 	; move value in rax to rbx 


