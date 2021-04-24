#include "doctest.h"
#include "lock_guards.hpp"

#include <iostream>
#include <thread>

using namespace learn;

TEST_CASE("basic interface")
{

  SUBCASE("flow 1 - xlock")
  {
    lockable l_1("lock_1");
    xlock xk_1(l_1);
  }

  SUBCASE("flow 2 - locks on same lock")
  {
    lockable l_1("lock_1");

    xlock xk_1(l_1);
    CHECK(xk_1.is_called());
    CHECK(xk_1.is_locked());

    xlock xk_2(l_1);
    CHECK(xk_2.is_called() == false);
    CHECK(xk_2.is_locked());

    slock sk_1(l_1);
    CHECK(sk_1.is_called()); // called for downgrade
    CHECK(sk_1.is_locked());

    slock sk_2(l_1);
    CHECK(sk_2.is_called() == false);
    CHECK(sk_2.is_locked());

    xlock xk_3(l_1);
    CHECK(xk_3.is_called()); // called for upgrade
    CHECK(xk_3.is_locked());

    xlock xk_4(l_1);
    CHECK(xk_4.is_called() == false);
    CHECK(xk_4.is_locked());
  }

  SUBCASE("flow 3 - locks on same lock nested")
  {
    lockable l_1("lock_1");

    {
      xlock xk_1(l_1);
      CHECK(xk_1.is_called());
      CHECK(xk_1.is_locked());

      {
        xlock xk_2(l_1);
        CHECK(xk_2.is_called() == false);
        CHECK(xk_2.is_locked());

        {
          slock sk_1(l_1);
          CHECK(sk_1.is_called()); // called for downgrade
          CHECK(sk_1.is_locked());

          {
            slock sk_2(l_1);
            CHECK(sk_2.is_called() == false); // re-enter
            CHECK(sk_2.is_locked());

            {
              xlock xk_3(l_1);
              CHECK(xk_3.is_called()); // called for upgrade
              CHECK(xk_3.is_locked());

              xlock xk_4(l_1);
              CHECK(xk_4.is_called() == false); // re-enter
              CHECK(xk_4.is_locked());
            }
          }
        }
      }

      // sk_1이 exit할 때 xk_2를 보고 다시 xlock 모드로 락
      CHECK(xk_1.is_called());
      CHECK(xk_1.is_locked());
    }
  }

  SUBCASE("flow 4 - alternating xlock and slock")
  {
    lockable l_1("lock_1");

    xlock xk_1(l_1);
    {
      slock sk_1(l_1); // downgrade

      CHECK(sk_1.is_locked());
      CHECK(sk_1.is_called());

      CHECK(xk_1.is_locked()); // though, downgraded
      CHECK(xk_1.is_called());

      {
        xlock xk2_1(l_1); // upgrade

        CHECK(xk2_1.is_locked());
        CHECK(xk2_1.is_called());
      }

      // recovered to sk_1

      CHECK(sk_1.is_locked()); // though, downgraded
      CHECK(sk_1.is_called());
    }

    // recovered to xk_1
    CHECK(xk_1.is_locked());
    CHECK(xk_1.is_called());
  }

  SUBCASE("flow 5 - downgrade and recover")
  {
    lockable l_1("lock_1");
    lockable l_2("lock_2");

    xlock xk_1(l_1);
    CHECK(xk_1.is_locked());
    CHECK(xk_1.is_called());

    {
      slock sk_1(l_1);
      CHECK(sk_1.is_locked());
      CHECK(sk_1.is_called());

      // enter slock region
      {
        slock sk_2(l_2);
        CHECK(sk_2.is_called());
        CHECK(sk_2.is_locked());

        xlock xk_2(l_2);
        CHECK(xk_2.is_called());
        CHECK(xk_2.is_locked());
      }
    }

    // recovered
    CHECK(xk_1.is_called());
    CHECK(xk_1.is_locked());
  }
}

TEST_CASE("lock between threads")
{
  SUBCASE("two threads on variables")
  {
    lockable l_1("lock_1");

    for (int j = 0; j < 100; ++j)
    {
      int v = 0;

      std::thread t1([&l_1, &v]() {
        for (int i = 0; i < 1000; ++i)
        {
          xlock x_1(l_1);
          v += 1;

          slock s_1(l_1); // downgrade
        }
        });

      auto func = [&l_1, &v]() {
        for (int i = 0; i < 1000; ++i)
        {
          int lv = 0;

          slock s_1(l_1);
          lv = v;           // 여러 쓰레드에서 실행할 경우, 여기서 이전 값을 같이 보게 된다.

          xlock x_1(l_1);
          v = ++lv;         // 하나의 쓰레드에서 먼저 실행하고, 이전 값을 본 쓰레드에서 덮어 쓴다. 이는 정상 동작이다.
        }
      };

      std::thread t2(func);

      t1.join();
      t2.join();

      CHECK(v == 2000);

      std::cout << "loop: " << j << std::endl;
    }
  }

  SUBCASE("more threads on variables")
  {
    lockable l_1("lock_1");

    for (int j = 0; j < 1; ++j)
    {
      int v = 0;

      std::thread t1([&l_1, &v]() {
        for (int i = 0; i < 1000; ++i)
        {
          xlock x_1(l_1);
          v += 1;
        }
        });

      std::thread t2([&l_1, &v]() {
        for (int i = 0; i < 1000; ++i)
        {
          int lv = 0;

          xlock x_1(l_1); 
          lv = v;
          v = ++lv;

          // downgrade는 변경 값을 정확하게 볼 수 있다.
          {
            slock s_1(l_1);
            CHECK(lv == v);
          }
        }
      });

      std::thread t3([&l_1, &v]() {
        for (int i = 0; i < 1000; ++i)
        {
          int lv = 0;

          xlock x_1(l_1); 
          lv = v;
          v = ++lv;

          {
            slock s_1(l_1);
            CHECK(lv == v);
          }
        }
      });

      std::thread t4([&l_1, &v]() {
        for (int i = 0; i < 1000; ++i)
        {
          int lv = 0;

          xlock x_1(l_1); 
          lv = v;
          v = ++lv;

          {
            slock s_1(l_1);
            CHECK(lv == v);
          }
        }
      });

      t1.join();
      t2.join();
      t3.join();
      t4.join();

      std::cout << "loop: " << j << std::endl;

      CHECK(v == 4000);

    }
  }
}

namespace {

class simple_object 
{
public: 
  simple_object()
    : v_{ 0 }
    , lock_{ "simple_object" }
    , slow_read_(false)
  {}

  void set_slow_read()
  {
    slow_read_ = true;
  }

  void add(int av)
  {
    xlock x(lock_);
    v_ += av;
  }

  int get() const
  {
    slock s(lock_); 
    if (slow_read_)
    {
      YieldProcessor();
    }
    return v_;
  }

  int get2() const
  {
    slock s(lock_);
    return v_ + 2;
  }

  lockable& lock()
  {
    return lock_;
  }

private:
  int v_;
  mutable lockable lock_;
  bool slow_read_;
};

class simple_object_mutex 
{
public: 
  simple_object_mutex()
    : v_{ 0 }
    , lock_{}
  {}

  void add(int av)
  {
    std::lock_guard x(lock_);
    v_ += av;
  }

  int get() const
  {
    std::lock_guard s(lock_); 
    return v_;
  }

  int get2() const
  {
    std::lock_guard s(lock_);
    return v_ + 2;
  }

private:
  int v_;
  mutable std::mutex lock_;
};

} // noname

TEST_CASE("code simulation")
{
  SUBCASE("simple object")
  {
    simple_object so;

    std::thread t1([&so]() {
      for (int i = 0; i < 100; ++i)
      {
        so.add(3);
      }
      });

    std::thread t2([&so]() {
      for (int i = 0; i < 100; ++i)
      {
        so.add(5);
      }
      });

    t1.join();
    t2.join();

    CHECK(so.get() == (3 * 100 + 5 * 100));
  }

  SUBCASE("handler")
  {
    simple_object so;

    xlock x(so.lock());

    so.add(3);                // re-enter
    CHECK(so.get() == 3);     // downgrade. keep xlock blocked from all threads
    CHECK(so.get2() == 5);    // re-enter

    // recovered xlock 
  }

  SUBCASE("upgrade/downgrade high contention")
  {
    simple_object so;

    constexpr int test_count = 10000000;

    auto start = std::chrono::steady_clock::now();

    std::thread t1([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        xlock x(so.lock());
        so.add(1);
        {
          slock s(so.lock()); // downgrade
          so.get();
        }
        // upgrade
      }
      });

    std::thread t2([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        xlock x(so.lock());
        so.add(1);
        {
          slock s(so.lock());
          so.get();
        }
      }
      });

    std::thread t3([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        slock s(so.lock());
        so.get();
        {
          xlock x(so.lock()); // upgrade
          so.add(1);
        }
        // downgrade
      }
      });

    std::thread t4([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        slock s(so.lock());
        so.get();
        {
          xlock x(so.lock());
          so.add(1);
        }
      }
      });

    t1.join();
    t2.join();
    t3.join();
    t4.join();

    CHECK(so.get() == test_count * 4);

    auto end = std::chrono::steady_clock::now();

    auto diff = end - start;
    auto ms = std::chrono::duration_cast<std::chrono::milliseconds>(diff).count();

    std::cout << "up/down. elapsed: " << ms << std::endl;
  }
}

TEST_CASE("performance")
{
  SUBCASE("performance - writer and reader")
  {
    simple_object so; 

    constexpr int test_count = 10000000;

    auto start = std::chrono::steady_clock::now();

    std::thread t1([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.add(3);
      }
      });

    std::thread t2([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.add(1);
      }
      });

    std::thread t3([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.get();
      }
      });

    std::thread t4([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.get();
      }
      });

    t1.join();
    t2.join();
    t3.join();
    t4.join();

    auto end = std::chrono::steady_clock::now();

    auto diff = end - start;
    auto ms = std::chrono::duration_cast<std::chrono::milliseconds>(diff).count();

    std::cout << "elapsed: " << ms << std::endl;

    // 4개 쓰레드, 1천만, 1456ms 
    // 4개 쓰레드, 1천만, 1172ms 
    // 4개 쓰레드, 1천만, 1160ms 
  }

  SUBCASE("performance - reader / writer with mutex")
  {
    simple_object_mutex so;

    constexpr int test_count = 10000000;

    auto start = std::chrono::steady_clock::now();

    std::thread t1([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.add(3);
      }
      });

    std::thread t2([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.add(1);
      }
      });

    std::thread t3([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.get();
      }
      });

    std::thread t4([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.get();
      }
      });

    t1.join();
    t2.join();
    t3.join();
    t4.join();

    auto end = std::chrono::steady_clock::now();

    auto diff = end - start;
    auto ms = std::chrono::duration_cast<std::chrono::milliseconds>(diff).count();

    std::cout << "elapsed: " << ms << std::endl;

    // 4개 쓰레드, 1천만, 1167ms 
    // 4개 쓰레드, 1천만, 1199ms 
    // 4개 쓰레드, 1천만, 1214ms 
  }

  SUBCASE("performance - writer and reader. slow read")
  {
    simple_object so;

    so.set_slow_read();

    constexpr int test_count = 10000000;

    auto start = std::chrono::steady_clock::now();

    std::thread t1([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.add(3);
      }
      });

    std::thread t2([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.add(1);
      }
      });

    std::thread t3([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.get();
      }
      });

    std::thread t4([&so, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        so.get();
      }
      });

    t1.join();
    t2.join();
    t3.join();
    t4.join();

    auto end = std::chrono::steady_clock::now();

    auto diff = end - start;
    auto ms = std::chrono::duration_cast<std::chrono::milliseconds>(diff).count();

    std::cout << "slow read. elapsed: " << ms << std::endl;

    // 1천만, 2245ms 
  }

  SUBCASE("performance - call overhead")
  {
    lockable lock("test");

    constexpr int test_count = 10000000;

    auto start = std::chrono::steady_clock::now();

    std::thread t1([&lock, test_count]() {
      for (int i = 0; i < test_count; ++i)
      {
        xlock x(lock);
        slock s(lock);
      }
      });

    t1.join();

    auto end = std::chrono::steady_clock::now();

    auto diff = end - start;
    auto ms = std::chrono::duration_cast<std::chrono::milliseconds>(diff).count();

    std::cout << "call overhead. elapsed: " << ms << std::endl;

    // 1천만, 283ms 
  }
}
