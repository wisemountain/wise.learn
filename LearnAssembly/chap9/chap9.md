# Chapter 9. Functions 

Yeah. C/C++과 어셈블리 간 양뱡향 호출이 되면 어셈블리 활용이 확 올라간다. 

## 9.1 The stack 

stack randomization to protect against malicous stack accesses. 

push 
pop

## 9.2 Call instruction 

call: 

- push return address 
- jump to the label 

## 9.3 Return instruction 

ret: 

- pops top of the stack as return address
- jump to the address

## 9.4 Function parameters and return value 

Linux ABI: 

- System V Application Binary Interface 


return value: 

- rax
- xmm0 

rsp should end in 0 

- means stack address is aligned to 16 bytes

First 6 integer parameters 

- Linux: 	rdi, rsi, rdx, rcx, r8, r9 
- Windows: 	rcx, rdx, r8, r9

Variable length function like scanf and printf : 

- rax has a number of floating point parameters 


### Linking with gcc 

gcc -o hello_world hello_world.o 

위와 같이 하면 표준 라이브러리와 링크하고 main을 참고하여 최종 빌드를 한다. 

## Stack frames

rbp가 스택 프레임 포인터 레지스터란 이름을 갖고 있고 Linux에서는 아직 그렇다. 

https://kuaaan.tistory.com/449

- x64 windows 호출 규약을 비교적 자세히 설명했다. 
- compiler의 코드 생성도 함께 보인다. 


함수 호출에 사용하는 레지스터들은 호출된 함수에서 값을 변경하지 않는다. 
그외 레지스터들은 값이 변경될 수 있는데 이를 보관해야 한다. 이런 과정을 
preserve 해야 한다고 한다. 


## Recursion 

factorial. 좋다. 이 정도는 이제 이해할 수 있다. 






