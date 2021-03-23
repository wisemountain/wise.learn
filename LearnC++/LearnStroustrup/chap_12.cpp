#include "../doctest.h"

#include <functional>
#include <string>
#include <string_view>
#include <iostream>
#include <vector>
#include <shared_mutex>

namespace
{
constexpr int factorial(int n)
{
  return (n > 1) ? n * factorial(n - 1) : 1;
}

} // anonymous

TEST_CASE("ss chap 12. Functions")
{
  SUBCASE("12.1.6 constexpr functions")
  {
    constexpr int f9 = factorial(9); // compile time calculation
    CHECK(f9 == factorial(9)); // runtime calculation
  }

  SUBCASE("recursive shared_mutex")
  {
    std::shared_mutex sm; 

    sm.lock_shared();
    sm.lock_shared();

    sm.unlock_shared(); 
    sm.unlock_shared();
  }

  SUBCASE("string_view")
  {
    std::string_view sv{ "Hello string_view" };
    CHECK(sv.length() == ::strlen("Hello string_view"));

    // https://docs.microsoft.com/en-us/cpp/c-language/storage-of-string-literals?view=msvc-160
    // - string literals have static storage duration. 
  }

  SUBCASE("{} initialization")
  {
    // std::initializer_list has a higher priority 
  }



  
}
