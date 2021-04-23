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
  {}

  void add(int av)
  {
    xlock x(lock_);
    v_ += av;
  }

  int get() const
  {
    slock s(lock_); 
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

    CHECK(so.get() == (3*100 + 5 * 100));
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
}
