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
    c1.f();   // noexcept가 release 모드 코드 생성에 특별한 영향이 없는 경우. stack frame을 유지
    c1.g();   // unwinding만 가능하면 되는 걸로 보인다. 
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

      // 00007FF729FA5080에 위 핸들러 루틴이 생성된다. 
      // _CxxCallException, __RaiseException으로 호출한다.
      //  __FrameHandler4::CxxCallCatchBlock 코드를 통해 원래 코드로 돌아온다.
      // catch에서 실행하는 코드가 상당히 많다. 
      // 따라서, exception은 "예외" 처리에 사용해야 한다. 
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
    // - _Iterator_base12(xmemory)에서 상속 
    // - 포인터로 메모리를 직접 이동한다. 

    // _Vector_iterator 
    // - _Vector_const_iterator를 base로 한다. 
    // - const_cast로 값을 변경할 수 있는 인터페이스를 추가한다. 
  }


}
