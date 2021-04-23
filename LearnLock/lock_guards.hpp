#pragma once 

#include "lockable.hpp"
#include "lock_thread_tracer.hpp"


#include <cassert>

namespace learn
{

class lock_guard_base
{
public:
  lock_guard_base() = default;

  bool is_locked() const
  {
    return lock_thread_tracer::inst.is_locked(index_);
  }

  bool is_called() const
  {
    return lock_thread_tracer::inst.is_called(index_);
  }

protected: 
  int8_t index_ = -1;
};

class xlock : public lock_guard_base
{
public: 
  xlock(lockable& lock)
    : lock_(lock)
  {
    index_ = lock_thread_tracer::inst.enter_xlock(&lock_);
  }

  ~xlock()
  {
    lock_thread_tracer::inst.exit_xlock(&lock_);
  }

private: 
  lockable& lock_;
};

class slock : public lock_guard_base
{
public: 
  slock(lockable& lock)
    : lock_(lock)
  {
    index_ = lock_thread_tracer::inst.enter_slock(&lock_);
  }

  ~slock()
  {
    lock_thread_tracer::inst.exit_slock(&lock_);
  }

private: 
  lockable& lock_;
};

} // learn