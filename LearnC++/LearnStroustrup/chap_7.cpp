#include "../doctest.h"

#include <string>
#include <iostream>
#include <vector>

namespace
{
} // anonymous

TEST_CASE("ss chap 7. Pointers, Arrays, and References")
{
  SUBCASE("7.3 arrays")
  {
    int aa[10]{};

    CHECK(aa[0] == 0);

    int x = aa[99]; // undefined behavior
    // CHECK(x != 0); // 메모리 상태에 따라 값이 다르게 된다. 

    SUBCASE("7.3.1 Array initializers")
    {
    }

    SUBCASE("7.3.2 String literals")
    {
      // char* p = "Plato"; // error in C++11 or higher 
      // "Plato" is const string literal 
      // It cannot be assigned to non-const char pointer

      char p[] = "Plato"; // This is possible

      std::string s = "Hello"
        " World";

      {
        std::string s = R"(\w\w)";
        CHECK(s == "\\w\\w");
      }
      {
        std::string s = R"("quoted string")";
        std::cout << s << std::endl;
      }
    }

  }

  SUBCASE("7.4 Pointer to arrays")
  {
    // The principle is to encapsulate in classes
    // and hide details. Then implement the detail efficient and correct. 
  }

  SUBCASE("7.5 Pointers and const")
  {
    const int model = 90; 
    // model = 200;

    char s[]{ "Gorm" };

    const char* pc = s;   // const char* pc == (const char)* 이다.
    // pc[3] = 'g'; // error

    char *const cp = s; // *const는 const 포인터이다. 그리고 declarator operator이다
    cp[3] = 'g';

    
  }

  SUBCASE("7.7 References")
  {
    const double& cdr{ 1 }; // create a temporary object and initialize with the lvalue of that object
    CHECK(cdr == 1);

    SUBCASE("7.7.2 rvalue references")
    {

    }
  }
}