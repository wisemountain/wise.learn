# Chapter 7. Bit operations 

## 7.1 Not operation 

mov 	rax, 0 
not 	rax 		; rax == 0xffffffffffffffff

## 7.2 And operation 

bitwise and 

mov 	rax, 0x12345678
mov 	rbx, rax
and 	rbx, 0xf 

## 7.3 Or operation 

bitwise or 

## 7.4 Xor operation 

## 7.5 Shift operation 

shl 
shr 

## 7.6 Bit testing and setting 

bt 
bts
btr

rflags: 

- CF : set to the value of the bit being tested

setc to get the CF 

setc dl

## 7.7 Extracting and filling a bit field


rol and ror 

비트 회전 연산이다. 



