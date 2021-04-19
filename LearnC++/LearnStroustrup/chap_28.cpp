#include "../doctest.h"

#include <functional>
#include <string>
#include <string_view>
#include <iostream>
#include <vector>
#include <shared_mutex>

namespace
{


template <typename T> 
class smart_pointer
{
public: 
  smart_pointer(T* vp)
    : vp_(vp)
  {}

  ~smart_pointer()
  {
    delete vp_;
  }

  T& operator*()
  {
    return *vp_;
  }

  typename std::enable_if<std::is_class<T>::value, T>::type* operator->()
  {
    return vp_;
  }

private:
  T* vp_;
};

class test_pointer
{
public: 
  void hello()
  {
    std::cout << "Hello" << std::endl;
  }
};

template <typename RESULT, typename OBJ, typename FUNC> 
RESULT call_shared(OBJ& obj, FUNC func)
{
  return func();
}

} // noname

TEST_CASE("ss chap 28. metaprogramming")
{
  SUBCASE("enable_if")
  {
    // smart_pointer<double> p{ new double{3.1} };
    // compile ����. SFINAE�� ó������ �����Ƿ� �� �˾ƾ� �Ѵ�. 
    // 

    // CHECK(*p == 3.1);

    smart_pointer<test_pointer> tp{new test_pointer};
    tp->hello();
  }

  SUBCASE("variadic templates")
  {
    // template parameter pack : ... args
    // parameter pack expansion  : args...

    // 

  }

  SUBCASE("function call")
  {
    int v = 0;

    call_shared<void>(v, []() {});
    auto r = call_shared<int>(v, [v]() { return v; });

    CHECK(r == 0);
  }

}
