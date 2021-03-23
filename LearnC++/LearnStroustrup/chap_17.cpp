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

    // �Ҹ����� assembly ���� �帧�� ��̷Ӵ�. 
    // �ſ� �� ��ƾ, CriticalSection �ϳ� ���� ����Ѵ�. 
    // delete[]�� delete�� �ᱹ delete�� ȣ���Ѵ�. 
    // ������ �޸𸮿� ���� üũ�� �Ϻ� ���Եȴ�. 

    // 
  }

  SUBCASE("default constructor")
  {
    Test2 t2;

    // call        std::basic_string<char,std::char_traits<char>,std::allocator<char> >::basic_string<char,std::char_traits<char>,std::allocator<char> > (07FF7F824D37Dh)  
    // - default constructor�� ��� ���� �ʱ�ȭ�� ȣ���ϵ��� �����ȴ�.
  }
}