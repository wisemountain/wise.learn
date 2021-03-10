# chapter 1 . Introduction 


## Why study assembly language? 

understanding how computer and compiler works. 

어셈블리에 능통하면 지금 하는 일인 프로그램의 검증에 상당한 도움이 된다. 지금은 대략 
읽을 수 있는 정도이지만 계속 막히는 부분이 있으므로 상당한 노력을 들여 완전하게 
인텔 어셈블리를 읽을 수 있는 수준까지 도달하도록 한다. 

Verification of Application code with assembly를 쓴다는 각오로 진행한다. 
Model checking과 함께 큰 한 축을 담당하게 될 것이다. 

실제로 windbg와 어셈블리로 라이브 디버깅을 진행하여 문제의 원인을 분석하고 찾은 경험은 
매우 소중하며 이와 같은 능력을 최대치로 만드는 것이 필요하다. 

도날드 커누쓰 님의 아트 오브 프로그래밍 책이 왜 어셈블리로 알고리즘 구현을 하는 것은 
문제의 본질을 보여주고자 함이다. 따라서, 컴퓨터 프로그램의 동작의 본질을 이해하려면 
꽤 자세하게 알고 있어야 한다. 

자유롭게 어셈블리를 읽는 것이 목표이다. 

## 1.2 What is a computer? 

### 1.2.1 Bytes 

### 1.2.2 Program execution 

## 1.3 Machine language

## 1.4 Assembly language

## 1.5 Assembling and linking 

```
yasm -f elf64 -g dwarf2 -l hello.lst hello.asm 
```
- elf64 
    - elf 64 bit format
- dwarf2 
    - debugging format 
- -l : listing file 

```
ld -o hello hello.o 
```
링크 

hello를 실행하니 int 0x80에서 segmentation fault가 난다. 

책에서 eax에 전달하는 exit 호출 값이 1이 아니라 0x60이다. 

yasm 사이트를 참고해서 진행한다. 어셈블리는 항상 조금씩 바뀐다. 












