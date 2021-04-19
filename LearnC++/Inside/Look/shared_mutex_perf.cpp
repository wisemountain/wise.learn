#include "../../doctest.h"

#include <cassert>
#include <iostream>
#include <mutex>
#include <shared_mutex>
#include <thread>
#include <vector>
#include <concurrent_unordered_map.h>

namespace
{

class test
{
public: 
  virtual void inc()
  {
    ++v;
  }

private:
  int v = 0;
};

void func(test* tv)
{
  tv->inc();
}

} // noname


TEST_CASE("sharef_mutex_perf")
{
  SUBCASE("single thread. shared mutex")
  {
    constexpr int test_count = 100000000;

    std::shared_mutex sm;

    std::chrono::steady_clock clock;

    auto start = clock.now();

    test tv;

    for (int i = 0; i < test_count; ++i)
    {
      std::shared_lock<std::shared_mutex> lock(sm);
      func(&tv);
    }

    auto end = clock.now();
    auto diff = end - start;

    std::cout << "elapsed: " << std::chrono::duration_cast<std::chrono::milliseconds>(diff).count() << " ms" << std::endl;;
  }

  SUBCASE("single thread. no mutex")
  {
    constexpr int test_count = 100000000;

    std::shared_mutex sm;

    std::chrono::steady_clock clock;

    auto start = clock.now();
    test tv;

    for (int i = 0; i < test_count; ++i)
    {
      func(&tv);
    }

    auto end = clock.now();
    auto diff = end - start;

    std::cout << "elapsed: " << std::chrono::duration_cast<std::chrono::milliseconds>(diff).count() << " ms" << std::endl;;
  }

  // 1¾ï¹ø. 
  //  - elapsed: 2213 vs. elapsed: 343 
  // 
}

TEST_CASE("shared mutex")
{
  SUBCASE("lock order")
  {
    std::shared_mutex sm;

    std::shared_lock<std::shared_mutex> slock(sm);

    std::cout << "slock" << std::endl;

    std::unique_lock<std::shared_mutex> xlock(sm);

    std::cout << "xlock" << std::endl;

  }
}
