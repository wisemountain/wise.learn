#include "../doctest.h"

#include <functional>
#include <string>
#include <string_view>
#include <iostream>
#include <vector>
#include <shared_mutex>

namespace
{

class Test1
{
public:
  explicit Test1(std::size_t size)
    : size_(size)
  {
    elems_ = new int[size_];
  }

  ~Test1()
  {
    delete[] elems_;
  }

private: 
  std::size_t size_;
  int* elems_;
};

class Test2
{

private: 
  std::string s_;
};

} // anonymous

TEST_CASE("ss chap 17. consturction, cleanup, copy and move")
{
  SUBCASE("explicit constructor")
  {
    // Test1 t1{ -1 }; // error C2398: Element '1': conversion from 'int' to 'size_t' requires a narrowing conversion

    // use explicit when applicable
  }

  SUBCASE("destructor")
  {
    Test1 t1{ 10 };

    // 소멸자의 assembly 실행 흐름이 흥미롭다. 
    // 매우 긴 루틴, CriticalSection 하나 등을 사용한다. 
    // delete[]와 delete는 결국 delete를 호출한다. 
    // 해제할 메모리에 대한 체크가 일부 포함된다. 

    // 
  }

  SUBCASE("default constructor")
  {
    Test2 t2;

    // call        std::basic_string<char,std::char_traits<char>,std::allocator<char> >::basic_string<char,std::char_traits<char>,std::allocator<char> > (07FF7F824D37Dh)  
    // - default constructor는 멤버 변수 초기화를 호출하도록 생성된다.
  }


}

