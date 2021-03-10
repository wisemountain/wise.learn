# Chapter 8. Branching and looping 

## 8.1 Unconditional jump 

jmp label 


local lables. 

test with jump.asm

## 8.2 Conditional jump 

cmp 

jCC: 

- jz 		je 				ZF=1
- jnz 		jne 			ZF=0
- jg 		jnle 			ZF=0, SF=0
- jge 		jnl 			SF=0
- jl 		jnge js 		SF=1
- jc 		jb jnae 		CF=1
- jnc 		jae jnb 		CF=0


### 8.2.1 Simple if statement 

```c
if ( a < b ) { 
	temp = a; 
	a = b; 
	b = temp; 
}
```

## 8.3 Looping with conditional jumps 


이제 본격적인 프로그래밍 구조에 대한 공부다. 좋다. 

```c
	sum = 0
	i = 0; 
	while ( i < 64 ) { 
		sum += data & i; 
		data = data >> 1; 
		i++; 
	}
```

Oxfedcba9876543210가 0b1111111011011100101110101001100001110110010101000011001000010000로 
32개의 1을 갖는다. 정확하게 계산한다. 


### 8.3.2 Do-whlie loops 

문자열에서 캐릭터 찾기로 간단한 예제를 보여준다. 


### 8.3.3 Counting loops 

```c
	for ( i = 0; i< n ; i++ ) {
		c[i] = a[i] + b[i]; 
	}
```

```asm
	mov rdx , [n] 				; rdx = n
	xor ecx, ecx 				; rcx = 0
for: 
	cmp rcx , rdx 				; 	
	je end_for					; if rcx == rdx (n) then goto end_for
	mov rax , [a+rcx*8] 		; rax = a[rcx]
	add rax , [b+rcx*8] 		; rax += b[rcx]
	mov [c+rcx*8] , rax 		; c[rcx] = rax
	inc rcx 					; ++rcx	
	jmp for						; goto for
end_for:
```

레지스터를 변수명으로 하여 C 코드로 변경하면 읽기 수월해진다. 
컴파일러가 만드는 코드는 훨씬 복잡하긴 하나 오히려 나중에는 C++보다 더 단순해 보일 것이다. 

## 8.4 Loop instructions 

loop 명령이 있긴 하나 느리다. 그런데 왜 있나? 

## 8.5 Repeat string (array) instructions 

### 8.5.1 string instructions 

movsb instruction moves bytes from the address specified by rsi to the address specified by rdi. 

```asm 
	lea 	rsi, 	[src] 
	lea 	rdi, 	[dst]
	mov 	rcx, 	100000
	rep 	movsb 
```

암묵적인 동작이 많은 명령어들이 섞여 있다. movsb는 rsi에서 rdi로 바이트를 옮긴다. 
movsb가 한번 실행되면 rsi와 rdi 값이 b만큼 증가한다. (b, w, d, q에 따라 값이 달라진다)
rep는 rcx에 설정된 카운트 만큼 실행한다. 

```asm 
	mov 	eax, 	1 
	mov 	ecx, 	1000000
	lea 	rdi, 	[dst]
	rep 	stosd 
```

stosd도 비슷하게 동작하고 방향이 rax에서 rdi이다. 
실행마다 rdi 메모리 위치가 증가하는 것도 동일하다. 


lea 명령어: 

- 주소 값을 저장한다. 
- mov는 값을 저장하는 것과 차이가 있다. 

```asm
int num = 10;
008F18B2  mov         dword ptr [num], 0Ah  // [num]주소에 0Ah(10)을 저장
int * ptr = &num;
008F18B9  lea         eax,[num]  // eax에 [num]주소 값을 저장
008F18BC  mov         dword ptr [ptr],eax  // [ptr]에 eax 값을 저장
*ptr = 12;
008F18C5  mov         eax,dword ptr [ptr]  // eax에 [ptr]의 값을 저장 ([num]주소 값이 저장됨)
008F18C8  mov         dword ptr [eax],0Ch  // eax 레지스터 자체에가 아닌, eax 값을 주소로 사용하여
					      메모리([num]주소)에 0Ch(12)을 저장
```

간단하고 좋은 예시이다. assembly에서 포인터 조작에 해당한다. 

``asm 
 		lea rsi , [src]
 		lea rdi , [dst]
 		mov ecx , 1000000

 more : 
		lodsb
 		cmp al , 13
 		je skip
 		stosb
 skip : 
 		sub ecx , 1
 		jnz more
```

```c 
	rsi = &src;
	rdi = &dst;
	rcx = 1000000;
	i = 0

more: 
	rax = rsi[0]; 					// loadsb
	if ( rax == 13 ) goto skip		// cmp al, 13 / je skip
	dst[i] = src[i] 				// stosb
	i++ 							// stosb for rsi / rdi address

skip: 
	--rcx; 							// sub ecx, 1
	if ( rcx != 0 ) goto more 		// jnz more
```


Scan:

```asm 
	segment . text
	global strlen
strlen : 
	cld 					; prepare to increment rdi
	mov rcx , -1 			; maximum number of iterations
	xor al , al 			; will scan for 0
	repne scasb 			; repeatedly scan for 0
	mov rax , -2 			; start at -1 , end 1 past the end. (-1)-1 
	sub rax , rcx 			; (-1)-1 - location
	ret
```

cld: clear direction flag. rflags의 DF 필드를 0으로 초기화 한다. 
std: set direction flag. rflags의 DF 필드를 1로 하여 감소 방향으로 설정한다. 

rdi에 첫 파라미터를 linux에서 전달한다. 

procedure를 다루게 되면 gdb로 레지스터 상태를 보면서 확인한다. 


Compare: 

```asm 
	segment . text
	global memcmp
memcmp : 
	mov rcx , rdx
	repe cmpsb 						; compare until end or difference
	cmp rcx , 0
	jz equal 						; reached the end
	movzx eax , byte [rdi-1]
	movzx ecx , byte [rsi-1]
	sub rax , rcx
	ret
equal: 
	xor eax , eax
	ret
```

movzx는 zero extend move. 


