#include "../doctest.h"

#include <functional>
#include <string>
#include <string_view>
#include <iostream>
#include <vector>
#include <shared_mutex>

namespace
{

class Case1
{
public: 

  void f() noexcept
  {
  }

  void g()
  {
  }
};

template <typename ValueType> 
class result
{
public: 
  result() 
    : v_{}
    , desc_{"success"}
  {}

  result(ValueType&& v, const char* desc)
    : v_{ v }
    , desc_{ desc }
  {}

  ValueType value() const
  {
    return v_;
  }

  const std::string_view& desc() const
  {
    return desc_;
  }

  operator bool() const
  {
    return v_ == ValueType{};
  }
  
private:
  ValueType v_;
  std::string_view desc_;
};

enum class error_code : uint16_t
{
  success,
  fail_1,
  end
};

enum class error_code_app : uint16_t
{
  fail_2 = error_code::end,
  end
};


#define RESULT_EC(ec) result<error_code>(ec, #ec)

} // anonymous

TEST_CASE("ss chap 13. Exceptions")
{
  SUBCASE("noexcept test")
  {
    Case1 c1;  
    c1.f();   // noexcept�� release ��� �ڵ� ������ Ư���� ������ ���� ���. stack frame�� ����
    c1.g();   // unwinding�� �����ϸ� �Ǵ� �ɷ� ���δ�. 
  }

  SUBCASE("result")
  {
    result<int> rc(0, "success");
    result<error_code> rc2(error_code::success, "success");
    auto rc3{ RESULT_EC(error_code::fail_1) };
    result<error_code> rc4;

    CHECK(!!rc);
    CHECK(rc2);
    CHECK(!rc3);
    CHECK(rc4);
  }

  SUBCASE("catch with ...")
  {
    try
    {
      std::string s{ "Hello" };
      throw std::exception("Hello Exception");
    }
    catch (...)
    {
      std::cout << "Exception" << std::endl;

      // 00007FF729FA5080�� �� �ڵ鷯 ��ƾ�� �����ȴ�. 
      // _CxxCallException, __RaiseException���� ȣ���Ѵ�.
      //  __FrameHandler4::CxxCallCatchBlock �ڵ带 ���� ���� �ڵ�� ���ƿ´�.
      // catch���� �����ϴ� �ڵ尡 ����� ����. 
      // ����, exception�� "����" ó���� ����ؾ� �Ѵ�. 
    }
  }

  SUBCASE("syntax")
  {
    // function body try block 
    // noexcept(noexcept)

  }

  SUBCASE("reading vector code")
  {
    // _Vector_const_iterator 
    // - _Iterator_base12(xmemory)���� ��� 
    // - �����ͷ� �޸𸮸� ���� �̵��Ѵ�. 

    // _Vector_iterator 
    // - _Vector_const_iterator�� base�� �Ѵ�. 
    // - const_cast�� ���� ������ �� �ִ� �������̽��� �߰��Ѵ�. 
  }


}
