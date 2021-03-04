

fbb(1024)

CreateString("Sword")
- len : 5
- PreAlign<uoffset_t>(6)
  - PreAlign(6, sizeof(uoffset_t) = 4)
    - PaddingBytes(GetSize()+6, 4) = PaddingBytes(0+6, 4) 
      - (~buf_size)+1 & (scalar_size-1) = (~6)+1 & (4-1) = 2

PaddingBytes:
// Computes how many bytes you'd have to pad to be able to write an
// "scalar_size" scalar if the buffer had grown to "buf_size" (downwards in
// memory).

   cur_와 scratch_ 두 가지가 있다. 
   

- vector_downward::fill(2) 
   - make_space(2) 
     - cur_[i]에 0으로 채운다. 
   - cur_는 쓸 곳이다. 
   - scratch_는 임시로 쓸 수 있는 buf_ 상의 공간이다. 
   - fill이 vector_downward의 핵심 함수이다. 
   
- vector_downward::push(uint8_t* bytes, size_t num) 
   - make_space()로 공간을 확보하고 
   - memcpy로 바이트를 넣는다. 






