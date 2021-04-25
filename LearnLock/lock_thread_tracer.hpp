#pragma once

#include "lockable.hpp"
#include "lock_tracer.hpp"
#include "fmt/format.h"

#include <array>
#include <cassert>
#include <cstdint>
#include <exception>

namespace learn
{

class lock_exception : public std::exception
{
public: 
  lock_exception(const char* what) 
    : std::exception(what)
  {}
};

class lock_thread_tracer
{
public: 
  static thread_local lock_thread_tracer inst;
  static constexpr int8_t max_lock_depth = 8;          // the maximum depth of nested lock 

private:
  lock_thread_tracer()
    : current_(-1)
    , locks_()
    , initialized_(false)
  {
  }

public:
  ~lock_thread_tracer()
  {
  }

  // xlock 모드로 락을 잡고, 락 인덱스를 돌려줌 
  int8_t enter_xlock(lockable* lock, bool keep)
  {
    init();

    lock_info* e_lock = nullptr;

    int8_t prev = -1;
    bool has_keep = false;    // 이전에 keep로 락한 락이 있는 지 확인.

    for ( int idx = current_; idx >=0; --idx)
    {
      if (locks_[idx].lock_ == lock)
      {
        e_lock = &locks_[idx];
        prev = idx;

        int8_t p_idx = prev;

        while (p_idx >= 0)
        {
          auto p_lock = &locks_[p_idx];
          if (p_lock->keep_)
          {
            assert(p_lock->type_ == lock_type::xlock);
            has_keep = true;
            break;
          }
          p_idx = p_lock->prev_;
        }

        break;
      }
    }

    if ((current_ + 1) >= max_lock_depth)
    {
      throw lock_exception(fmt::format("max lock depth reached in enter_xlock. current:{}", current_).c_str());
      return -1;
    }

    int8_t idx = current_ + 1;

    lock_info* n_lock = &locks_[idx];
    n_lock->lock_ = lock;
    n_lock->type_ = lock_type::xlock;
    n_lock->locked_ = true;
    n_lock->prev_ = prev;
    n_lock->next_ = -1;
    n_lock->keep_ = keep;

    if (e_lock == nullptr)
    {
      lock->lock();
      n_lock->called_ = true;
    }
    else
    {
      e_lock->next_ = idx;

      if (has_keep)
      {
        n_lock->called_ = false;
      }
      else
      {
        switch (e_lock->type_)
        {
        case lock_type::xlock:
        {
          // reentrant
          n_lock->called_ = false;
        }
        break;
        case lock_type::slock:
        {
          // upgrade 
          e_lock->lock_->upgrade();

          // e_lock is still locked by following 

          n_lock->called_ = true;
        }
        break;
        }
      }
    }

    // lock is in xlock state 

    ++current_;
    n_lock->invalid_ = false;

    return current_;
  }

  void exit_xlock(lockable* lock)
  {
    assert(current_ >= 0);

    auto current = &locks_[current_];

    assert(current != nullptr);
    assert(current->lock_ == lock);
    assert(current->type_ == lock_type::xlock);
    assert(current->locked_);
    assert(current->invalid_ == false);

    if (current->called_)
    {
      if (current->prev_ < 0)
      {
        current->lock_->unlock();
      }
      else 
      {
        assert(current_ > current->prev_);

        auto prev_lock = &locks_[current->prev_];

        switch (prev_lock->type_)
        {
        case lock_type::xlock:
        {
          assert(!"unrechable state since same lock is re-entered");
          throw lock_exception("unreachable state in exit_xlock");
        }
        break;
        case lock_type::slock:
        {
          // 호출 여부와 관계 없이 락을 복원한다. 
          // 호출 상태는 유지하여 원래 호출한 락에서 exit처리를 한다.
          prev_lock->lock_->downgrade();
          prev_lock->locked_ = true;
        }
        break;
        }
      }
    }

    --current_;

    reset_lock(current);
  }

  // slock_keep 모드로 락을 잡고, 락 인덱스를 돌려줌 
  int8_t enter_slock(lockable* lock)
  {
    init();

    lock_info* e_lock = nullptr;
    int8_t prev = -1;
    bool has_keep = false;    // 이전에 keep로 락한 락이 있는 지 확인.

    for (int idx = current_; idx >= 0; --idx)
    {
      if (locks_[idx].lock_ == lock)
      {
        e_lock = &locks_[idx];
        prev = idx;

        int8_t p_idx = prev;

        while (p_idx >= 0)
        {
          auto p_lock = &locks_[p_idx];
          if (p_lock->keep_)
          {
            assert(p_lock->type_ == lock_type::xlock);
            has_keep = true;
            break;
          }
          p_idx = p_lock->prev_;
        }

        break;
      }
    }

    if ((current_ + 1) >= max_lock_depth)
    {
      throw lock_exception(fmt::format("max lock depth reached in enter_slock. current:{}", current_).c_str());
      return -1;
    }

    int8_t idx = current_ + 1;

    lock_info* n_lock = &locks_[idx];
    n_lock->lock_ = lock;
    n_lock->type_ = lock_type::slock;
    n_lock->locked_ = true;
    n_lock->prev_ = prev;
    n_lock->next_ = -1;

    if (e_lock == nullptr)
    {
      lock->lock_shared();
      n_lock->called_ = true;
    }
    else
    {
      e_lock->next_ = idx;

      if (has_keep)
      {
        n_lock->called_ = false;
      }
      else
      {
        switch (e_lock->type_)
        {
        case lock_type::xlock:
        {
          // downgrade 
          e_lock->lock_->downgrade();

          // e_lock is still locked by following 

          n_lock->called_ = true;
        }
        break;
        case lock_type::slock:
        {
          // reentrant 
          n_lock->called_ = false;
        }
        break;
        }
      }
    }

    // lock is in slock state 

    ++current_;
    n_lock->invalid_ = false;

    return current_;
  }

  void exit_slock(lockable* lock)
  {
    assert(current_ >= 0);

    auto current = &locks_[current_];

    assert(current != nullptr);
    assert(current->lock_ == lock);
    assert(current->type_ == lock_type::slock);
    assert(current->locked_);
    assert(current->invalid_ == false);

    if (current->called_)
    {
      if (current->prev_ < 0)
      {
        current->lock_->unlock_shared();
      }
      else 
      { 
        assert(current_ > current->prev_);

        auto prev_lock = &locks_[current->prev_];

        switch (prev_lock->type_)
        {
        case lock_type::xlock:
        {
          // 호출 여부와 관계 없이 락을 복원한다. 
          // 호출 상태는 유지하여 원래 호출한 락에서 exit처리를 한다.
          prev_lock->lock_->upgrade();
          prev_lock->locked_ = true;
        }
        break;
        case lock_type::slock:
        {
          assert(!"unrechable state since same lock is re-entered");
          throw lock_exception("unreachable state in exit_slock");
        }
        break;
        }
      }
    }

    --current_;

    reset_lock(current);
  }

  bool has_lock(int8_t idx) const
  {
    return idx <= current_;
  }

  bool is_locked(int8_t idx) const
  {
    assert(idx <= current_);
    return locks_[idx].locked_;
  }

  bool is_called(int8_t idx) const
  {
    assert(idx <= current_);
    return locks_[idx].called_;
  }

  const std::thread::id get_thread_id() const
  {
    return id_;
  }

  std::string to_string()
  {
    std::string result; 

    for (int i = 0; i < current_; ++i)
    {
      auto lock = &locks_[i];
      auto info = fmt::format(
        "<index:{}, lock:{}, type:{}, prev:{}, next:{}, locked:{}, called:{}, keep:{}>\r\n",
        i, 
        (void*)lock->lock_,
        lock->type_ == lock_type::xlock ? "xlock" : "slock", 
        lock->prev_, 
        lock->next_, 
        lock->locked_ ? 1 : 0, 
        lock->called_ ? 1 : 0, 
        lock->keep_ ? 1 : 0);

      result.append(info);
    }

    if (result.length() == 0)
    {
      return "<>";
    }

    return result;
  }

private: 
  void init()
  {
    if (!initialized_)
    {
      initialized_ = true;

      id_ = std::this_thread::get_id();
      lock_tracer::inst.add(this);
    }
  }


private: 
  enum class lock_type : uint8_t
  {
    xlock, 
    slock, 
  };

  // lock_info. note: keep the size small to keep the whole data in cache
  struct lock_info
  {
    lockable* lock_;
    int8_t  prev_;
    int8_t  next_;
    lock_type type_ : 1;
    bool locked_    : 1;
    bool called_    : 1;
    bool keep_      : 1;
    bool invalid_   : 1;

    lock_info()
      : lock_(nullptr)
      , prev_(-1)
      , type_()
      , locked_(false)
      , called_(false)
      , keep_(false)
      , invalid_(true)
    {
    }
  };

  void reset_lock(lock_info* lock)
  {
    lock->lock_ = nullptr; 
    lock->prev_ = -1;
    lock->next_ = -1;
    lock->locked_ = false;
    lock->called_ = false;
    lock->keep_ = false;
    lock->invalid_ = true;
  }

private: 
  std::array<lock_info, max_lock_depth> locks_;       // used array for cache coherence
  int current_;
  bool initialized_;
  std::thread::id id_;
};

} // learn



