#include "../doctest.h"

#include <functional>
#include <string>
#include <iostream>
#include <vector>

namespace
{
class Holder 
{
public: 
  std::function<int()> get(int v)
  {
    v_ = v;
    return [this]() {
      return value();
    };
  }

  int value() const
  {
    return v_;
  }

private:
  int v_;
};
} // anonymous

TEST_CASE("ss chap 11. Select operations")
{
  SUBCASE("11.4.3.3 Lambda and this")
  {
    std::function<int()> f;

    // this lifetime
    {
      Holder* h = new Holder(); 
      f = h->get(3);
      ::memset(h, 0, sizeof(Holder));
      delete h;
    }

    f();
    // CHECK(f() == 3);  // undefined behavior 
  }

  SUBCASE("11.5.2 Named casts")
  {
    Holder* p = reinterpret_cast<Holder*>(0xff00);
    // p->value(); // exception in debug mode. code is optimized and removed in relase mode
  }
}
