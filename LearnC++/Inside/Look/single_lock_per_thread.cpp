#include "../../doctest.h"

#include <cassert>
#include <mutex>
#include <thread>
#include <vector>

namespace
{

class lock_trace
{
public: 
  lock_trace()
  {
  }

  void enter(std::recursive_mutex* lock)
  {
    std::lock_guard<std::recursive_mutex> guard(trace_lock_);

    if (!locks_.empty())
    {
      std::recursive_mutex* last = locks_.back();
      assert(last != nullptr);
      last->unlock();
    }

    locks_.push_back(lock);
  }

  void exit(std::recursive_mutex* lock)
  {
    std::lock_guard<std::recursive_mutex> guard(trace_lock_);

    assert(locks_.empty() == false);

    locks_.pop_back();

    if (!locks_.empty())
    {
      std::recursive_mutex* last = locks_.back();
      assert(last != nullptr);
      last->lock();
    }
  }

  size_t count() const
  {
    return locks_.size();
  }

private: 
  std::recursive_mutex trace_lock_;
  std::vector<std::recursive_mutex*> locks_;
};

class lock_trace_guard
{
public: 
  lock_trace_guard(lock_trace& trace, std::recursive_mutex& lock)
    : trace_(trace)
    , lock_(lock)
  {
    trace_.enter(&lock_); // when enter, unlock last lock (and all other locks)
    lock_.lock();
  }

  ~lock_trace_guard()
  {
    lock_.unlock();      
    trace_.exit(&lock_);  // when exit, lock last lock only
  }

private:
  lock_trace& trace_;
  std::recursive_mutex& lock_;
};

static thread_local lock_trace tracer;

} // noname


TEST_CASE("single lock per thread")
{
  SUBCASE("attempt 1")
  {
    std::recursive_mutex lock1;

    int v = 0;
    int b = 1;

    std::thread t1([&v, &lock1, &b]() {
      std::recursive_mutex local_mutex;
      lock_trace_guard lock_local(tracer, local_mutex);
      // locked with local_mutex

      for (int i = 0; i < 10; ++i)
      {
        lock_trace_guard lock(tracer, lock1);
        // locked with lock1 only
        b = 2;
        v += b;
      }

      // locked with local mutex
      });

    std::thread t2([&v, &lock1, &b]() {
      std::recursive_mutex local_mutex;
      lock_trace_guard lock_local(tracer, local_mutex);
      // locked with local mutex

      for (int i = 0; i < 10; ++i)
      {
        lock_trace_guard lock(tracer, lock1);
        // locked with lock1 only
        b = 1;
        v += b;
      }
      // locked with local mutex
      });

    t1.join();
    t2.join();

    CHECK(v == 20 + 10);
  }
}
