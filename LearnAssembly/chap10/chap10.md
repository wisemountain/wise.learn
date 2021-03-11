# Chapter 10. Arrays 

낮은 주소에서 높은 주소로 인덱스와 항목 크기에 따라 증가한다. 

- base + i * m ; base가 시작 주소, i가 인덱스, m이 항목 크기 


indexing.asm: 

- p a[1]이 동작하지 않는다. 
- 저자가 ygcc, ygdb를 만들었다고 한다. 
 

## 10.2 General pattern for memory references 

[label] 
[label + 2 * ind]
[reg]
[reg + k * ind]
[label + reg + k * ind]
[number + reg + k * ind] 


## 10.3 Allocating arrays 


## 10.5 Command line parameter array 



