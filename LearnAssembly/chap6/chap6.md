# Chapter 6. A little bit of math 

## 6.1 Negation 

Two's complement of its operand. 

 - one's complement and add 1 

neg 	rax 		; negate the value in rax 
neg 	dword [x] 	; negate the 4 byte value in x
neg 	byte [x] 	; negate the byte at x 


## 6.2 Addition 

add 	[a], rax	; add rax to a (a is the destination)

rflags: 

- OF 
- SF 
- ZF is set if the result is zero 

inc 	[a]

## 6.3 Substraction 

sub 	rsp, 16 

rflags: 

- OF
- SF 
- ZF


## 6.4 Multiplication 

```
imul 	qword [data] 		; multiply rax by data 
mov		[high], rdx 		; store upper part of product 
mov 	[low], rax			; store lower part of product 
```

```
imul 	rax, 100			; multiply rax by 100 and save it at rax 
imul 	r8, [x]				; multiply rax by x 
imul 	r9, r10 			; multiply r9 by r10 
```

```
imul 	rbx, [x], 100 		; store 100*x in rbx 
imul 	rdx, rbx, 50 		;  store 50*rbx in rdx
```

rflags: 

- CF 
- OF 


## 6.5 Division 

mov 	rax, [x]
mov 	rdx, 0
idiv 	[y] 			; divide rdx:rax by y
mov 	[quot], rax 	; store the quotient 
mov 	[rem], rdx 		; store the remainder

이보다 내용이 더 많다. 필요할 때 더 볼 것.

## 6.6 Conditional move instructions

cmovz 	; move if zero flag set 
cmovnz 	; move if zero flag not set 
cmovl 	; move if result was negative (less than)
cmovle 	; move if result was le (less than or equal)
cmovg 
cmovge 


## 6.7 Why move to a register 



