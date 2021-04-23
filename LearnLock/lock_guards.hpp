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

class xlock_keep : public lock_guard_base
{
public: 
  xlock_keep(lockable* lock)
    : lock_(lock)
  {
    assert(lock_ != nullptr);

    index_ = lock_thread_tracer::inst.enter_xlock_keep(lock_);
  }

  ~xlock_keep()
  {
    lock_thread_tracer::inst.exit_xlock_keep(lock_);
  }

private: 
  lockable* lock_;
};

class slock_keep : public lock_guard_base
{
public: 
  slock_keep(lockable* lock)
    : lock_(lock)
  {
    assert(lock_ != nullptr);

    index_ = lock_thread_tracer::inst.enter_slock_keep(lock_);
  }

  ~slock_keep()
  {
    lock_thread_tracer::inst.exit_slock_keep(lock_);
  }

private: 
  lockable* lock_;
};

class xlock_solo : public lock_guard_base
{
public: 
  xlock_solo(lockable* lock)
    : lock_(lock)
  {
    assert(lock_ != nullptr);

    index_ = lock_thread_tracer::inst.enter_xlock_solo(lock_);
  }

  ~xlock_solo()
  {
    lock_thread_tracer::inst.exit_xlock_solo(lock_);
  }

private: 
  lockable* lock_;
};

class slock_solo : public lock_guard_base
{
public: 
  slock_solo(lockable* lock)
    : lock_(lock)
  {
    assert(lock_ != nullptr);

    index_ = lock_thread_tracer::inst.enter_slock_solo(lock_);
  }

  ~slock_solo()
  {
    lock_thread_tracer::inst.exit_slock_solo(lock_);
  }

private: 
  lockable* lock_;
};

} // learn