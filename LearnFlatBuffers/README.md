

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

   cur_�� scratch_ �� ������ �ִ�. 
   

- vector_downward::fill(2) 
   - make_space(2) 
     - cur_[i]�� 0���� ä���. 
   - cur_�� �� ���̴�. 
   - scratch_�� �ӽ÷� �� �� �ִ� buf_ ���� �����̴�. 
   - fill�� vector_downward�� �ٽ� �Լ��̴�. 
   
- vector_downward::push(uint8_t* bytes, size_t num) 
   - make_space()�� ������ Ȯ���ϰ� 
   - memcpy�� ����Ʈ�� �ִ´�. 






