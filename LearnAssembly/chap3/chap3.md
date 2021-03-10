# Chapter 3. Computer memory 

## Memory mapping 

- page 
- hardware mapping of pages for each process 

## 3.2 Process memory model in Linux 

- text
    - 0x400000
- data 
    - text 바로 위 
- heap 
- stack 
    - begins at 0x7fffffffffff
    - 16MB로 제한 
    
logical address가 48bit이다. 상위 12비트가 페이지이다. 


## 3.3 Memory example 

dd : data, double word (4 bytes)
dw : data, word (2 bytes)
times : array count 
db : data, byte 

dumping memory 

- p expression 
- x/NFS address






