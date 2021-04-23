#include "doctest.h"
#include "lock_guards.hpp"

using namespace learn;

TEST_CASE("basic interface")
{

  SUBCASE("flow 1 - xlock_keep")
  {
    lockable l_1("lock_1");
    xlock_keep xk_1(&l_1);
  }

  SUBCASE("flow 2 - two xlock_keep")
  {
    lockable l_1("lock_1");
    lockable l_2("lock_2");

    xlock_keep xk_1(&l_1);
    xlock_keep xk_2(&l_2);
  }

  SUBCASE("flow 3 - xlock_keep and slock_keep")
  {
    lockable l_1("lock_1");
    xlock_keep xk_1(&l_1);
    slock_keep sk_1(&l_1);
  }

  SUBCASE("flow 4 - alternating xlock_keep and slock_keep")
  {
    lockable l_1("lock_1");

    xlock_keep xk_1(&l_1);
    {
      slock_keep sk_1(&l_1); // downgrade

      CHECK(sk_1.is_locked());
      CHECK(sk_1.is_called());

      CHECK(xk_1.is_locked() == false);
      CHECK(xk_1.is_called() == false);

      {
        xlock_keep xk2_1(&l_1); // upgrade

        CHECK(xk2_1.is_locked());
        CHECK(xk2_1.is_called());

        CHECK(sk_1.is_locked() == false);
        CHECK(sk_1.is_called() == false);
      }

      // recovered to sk_1

      CHECK(sk_1.is_locked());
      CHECK(sk_1.is_called());
    }

    // recovered to xk_1
    CHECK(xk_1.is_locked());
    CHECK(xk_1.is_called());
  }

  SUBCASE("flow 5 - xlock_solo")
  {
    lockable l_1("lock_1");
    xlock_solo xs_1(&l_1);

    CHECK(xs_1.is_called());
    CHECK(xs_1.is_locked());
  }

  SUBCASE("flow 6 - xlock_solo after keep locks")
  {
    lockable l_1("lock_1");

    xlock_keep xk_1(&l_1);
    CHECK(xk_1.is_locked());
    CHECK(xk_1.is_called());

    // enter solo lock region
    {
      xlock_solo xs_1(&l_1);
      CHECK(xs_1.is_called());
      CHECK(xs_1.is_locked());

      CHECK(xk_1.is_called());
      CHECK(xk_1.is_locked() == false);
    }

    // recover to xk_1
    CHECK(xk_1.is_called());
    CHECK(xk_1.is_locked());
  }

  SUBCASE("flow 7 - solo downgrade and recover")
  {
    lockable l_1("lock_1");
    lockable l_2("lock_2");

    xlock_keep xk_1(&l_1);
    CHECK(xk_1.is_locked());
    CHECK(xk_1.is_called());

    // enter solo lock region
    {
      xlock_solo xs_1(&l_2);
      CHECK(xs_1.is_called());
      CHECK(xs_1.is_locked());

      CHECK(xk_1.is_called());
      CHECK(xk_1.is_locked() == false);

      // enter shared lock region
      {
        // downgrade
        slock_solo ss_1(&l_2);
        CHECK(ss_1.is_called());
        CHECK(ss_1.is_locked());

        CHECK(xs_1.is_called());
        CHECK(xs_1.is_locked() == false);
      }
      // recover to xs_1
      // ~ss_1, xk_1, xs_1 순으로 실행 (~는 unlock. 없으면 lock) 
    }

    // recover to xk_1
    CHECK(xk_1.is_called());
    CHECK(xk_1.is_locked());
  }

  SUBCASE("flow 8 - solo upgrade and recover")
  {
    lockable l_1("lock_1");
    lockable l_2("lock_2");

    xlock_keep xk_1(&l_1);
    CHECK(xk_1.is_locked());
    CHECK(xk_1.is_called());

    // enter solo lock region
    {
      slock_solo ss_1(&l_2);
      CHECK(ss_1.is_called());
      CHECK(ss_1.is_locked());

      CHECK(xk_1.is_called());
      CHECK(xk_1.is_locked() == false);

      // enter shared lock region
      {
        // upgrade
        xlock_solo xs_1(&l_2);
        CHECK(xs_1.is_called());
        CHECK(xs_1.is_locked());

        CHECK(ss_1.is_called());
        CHECK(ss_1.is_locked() == false);
      }
      // recover to xs_1
      // ~xs_1, xk_1, ss_1 순으로 실행 (~는 unlock. 없으면 lock) 
    }

    // recover to xk_1
    CHECK(xk_1.is_called());
    CHECK(xk_1.is_locked());
  }
}

TEST_CASE("producer / consumer locking")
{

}

TEST_CASE("multiple read / write access")
{

}