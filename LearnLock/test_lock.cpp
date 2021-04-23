#include "doctest.h"
#include "lock_guards.hpp"

using namespace learn;

TEST_CASE("basic interface")
{

  SUBCASE("flow 1 - xlock")
  {
    lockable l_1("lock_1");
    xlock xk_1(&l_1);
  }

  SUBCASE("flow 2 - locks on same lock")
  {
    lockable l_1("lock_1");

    xlock xk_1(&l_1);
    CHECK(xk_1.is_called());
    CHECK(xk_1.is_locked());

    xlock xk_2(&l_1);
    CHECK(xk_2.is_called() == false);
    CHECK(xk_2.is_locked());

    slock sk_1(&l_1);
    CHECK(sk_1.is_called()); // called for downgrade
    CHECK(sk_1.is_locked());

    slock sk_2(&l_1);
    CHECK(sk_2.is_called() == false);
    CHECK(sk_2.is_locked());

    xlock xk_3(&l_1);
    CHECK(xk_3.is_called()); // called for upgrade
    CHECK(xk_3.is_locked());

    xlock xk_4(&l_1);
    CHECK(xk_4.is_called() == false);
    CHECK(xk_4.is_locked());
  }

  SUBCASE("flow 3 - locks on same lock nested")
  {
    lockable l_1("lock_1");

    {
      xlock xk_1(&l_1);
      CHECK(xk_1.is_called());
      CHECK(xk_1.is_locked());

      {
        xlock xk_2(&l_1);
        CHECK(xk_2.is_called() == false);
        CHECK(xk_2.is_locked());

        {
          slock sk_1(&l_1);
          CHECK(sk_1.is_called()); // called for downgrade
          CHECK(sk_1.is_locked());

          {
            slock sk_2(&l_1);
            CHECK(sk_2.is_called() == false); // re-enter
            CHECK(sk_2.is_locked());

            {
              xlock xk_3(&l_1);
              CHECK(xk_3.is_called()); // called for upgrade
              CHECK(xk_3.is_locked());

              xlock xk_4(&l_1);
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

    xlock xk_1(&l_1);
    {
      slock sk_1(&l_1); // downgrade

      CHECK(sk_1.is_locked());
      CHECK(sk_1.is_called());

      CHECK(xk_1.is_locked()); // though, downgraded
      CHECK(xk_1.is_called());

      {
        xlock xk2_1(&l_1); // upgrade

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

    xlock xk_1(&l_1);
    CHECK(xk_1.is_locked());
    CHECK(xk_1.is_called());

    {
      slock sk_1(&l_1);
      CHECK(sk_1.is_locked());
      CHECK(sk_1.is_called());

      // enter slock region
      {
        slock sk_2(&l_2);
        CHECK(sk_2.is_called());
        CHECK(sk_2.is_locked());

        xlock xk_2(&l_2);
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

    
  }
}

TEST_CASE("multiple read / write access")
{

}