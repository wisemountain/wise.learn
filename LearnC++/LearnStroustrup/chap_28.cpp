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

}

TEST_CASE("ss chap 28. metaprogramming")
{
  SUBCASE("enable_if")
  {
    // smart_pointer<double> p{ new double{3.1} };
    // compile 에러. SFINAE로 처리되지 않으므로 더 알아야 한다. 
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

}
