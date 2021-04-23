#pragma once

#include "lockable.hpp"

#include <array>
#include <cassert>
#include <cstdint>

namespace learn
{

class lock_thread_tracer
{
public: 
  static thread_local lock_thread_tracer inst;
  static constexpr int8_t max_lock_depth = 10;          // the maximum depth of nested lock 

public:
  lock_thread_tracer()
    : current_(-1)
    , locks_()
  {
  }

  ~lock_thread_tracer()
  {
  }

  // xlock_keep 모드로 락을 잡고, 락 인덱스를 돌려줌 
  int8_t enter_xlock_keep(lockable* lock)
  {
    lock_info* e_lock = nullptr;

    int8_t prev = -1;

    for ( int idx = current_; idx >=0; --idx)
    {
      if (locks_[idx].lock_ == lock)
      {
        e_lock = &locks_[idx];
        prev = idx;
        break;
      }
    }

    if ((current_ + 1) >= max_lock_depth)
    {
      // throw exception
      return -1;
    }

    int8_t idx = current_ + 1;

    lock_info* n_lock = &locks_[idx];
    n_lock->lock_ = lock;
    n_lock->type_ = lock_type::xlock_keep;
    n_lock->locked_ = true;
    n_lock->prev_ = prev;
    n_lock->next_ = -1;

    if (e_lock == nullptr)
    {
      lock->lock();
      n_lock->called_ = true;
    }
    else
    {

      e_lock->next_ = idx;

      switch (e_lock->type_)
      {
      case lock_type::xlock_keep:
      {
        // reentrant
        n_lock->called_ = false;
      }
      break;
      case lock_type::slock_keep:
      {
        // upgrade 
        e_lock->lock_->unlock_shared();
        e_lock->locked_ = false; 
        e_lock->called_ = false; 

        n_lock->lock_->lock();
        n_lock->called_ = true;
      }
      break;
      case lock_type::xlock_solo:
      {
        if (e_lock->locked_)
        {
          // reentrant
          n_lock->called_ = false;
        }
        else
        {
          lock->lock();
          n_lock->called_ = true;
        }
      }
      break;
      case lock_type::slock_solo:
      {
        if (e_lock->locked_)
        {
          // upgrade 
          e_lock->lock_->unlock_shared();
          e_lock->locked_ = false; 
          e_lock->called_ = false; 

          n_lock->lock_->lock();
          n_lock->called_ = true;
        }
        else
        {
          lock->lock();
          n_lock->called_ = true;
        }
      }
      break;
      }
    }

    // lock is in xlock state 

    ++current_;
    n_lock->invalid_ = false;

    return current_;
  }

  void exit_xlock_keep(lockable* lock)
  {
    assert(current_ >= 0);

    auto current = &locks_[current_];

    assert(current != nullptr);
    assert(current->lock_ == lock);
    assert(current->type_ == lock_type::xlock_keep);
    assert(current->locked_);
    assert(current->invalid_ == false);

    if (current->called_)
    {
      current->lock_->unlock();

      if (current->prev_ >= 0)
      {
        assert(current_ > current->prev_);

        auto prev_lock = &locks_[current->prev_];

        switch (prev_lock->type_)
        {
        case lock_type::xlock_keep:
        case lock_type::xlock_solo:
        {
          assert(!"unrechable state since same lock is re-entered");
        }
        break;
        case lock_type::slock_keep:
        case lock_type::slock_solo:
        {
          prev_lock->lock_->lock_shared();
          prev_lock->called_ = true;
          prev_lock->locked_ = true;
        }
        break;
        }
      }
    }
    current->locked_ = false;
    current->called_ = false;
    current->invalid_ = true;

    --current_;
  }

  // slock_keep 모드로 락을 잡고, 락 인덱스를 돌려줌 
  int8_t enter_slock_keep(lockable* lock)
  {
    lock_info* e_lock = nullptr;
    int8_t prev = -1;

    for (int idx = current_; idx >= 0; --idx)
    {
      if (locks_[idx].lock_ == lock)
      {
        e_lock = &locks_[idx];
        prev = idx;
        break;
      }
    }

    if ((current_ + 1) >= max_lock_depth)
    {
      // throw exception
      return -1;
    }

    int8_t idx = current_ + 1;

    lock_info* n_lock = &locks_[idx];
    n_lock->lock_ = lock;
    n_lock->type_ = lock_type::slock_keep;
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

      switch (e_lock->type_)
      {
      case lock_type::xlock_keep:
      {
        // downgrade 
        e_lock->lock_->unlock();
        e_lock->locked_ = false;
        e_lock->called_ = false;

        n_lock->lock_->lock_shared();
        n_lock->called_ = true;
      }
      break;
      case lock_type::slock_keep:
      {
        // reentrant 
        n_lock->called_ = false;
      }
      break;
      case lock_type::xlock_solo:
      {
        if (e_lock->locked_)
        {
          // downgrade 
          e_lock->lock_->unlock();
          e_lock->locked_ = false;
          e_lock->called_ = false;

          n_lock->lock_->lock_shared();
          n_lock->called_ = true;
        }
        else
        {
          lock->lock_shared();
          n_lock->called_ = true;
        }
      }
      break;
      case lock_type::slock_solo:
      {
        if (e_lock->locked_)
        {
          // reentrant
          n_lock->called_ = false;
        }
        else
        {
          lock->lock_shared();
          n_lock->called_ = true;
        }
      }
      break;
      }
    }

    // lock is in slock state 

    ++current_;
    n_lock->invalid_ = false;

    return current_;
  }

  void exit_slock_keep(lockable* lock)
  {
    assert(current_ >= 0);

    auto current = &locks_[current_];

    assert(current != nullptr);
    assert(current->lock_ == lock);
    assert(current->type_ == lock_type::slock_keep);
    assert(current->locked_);
    assert(current->invalid_ == false);

    if (current->called_)
    {
      current->lock_->unlock_shared();

      if (current->prev_ >= 0)
      {
        assert(current_ > current->prev_);

        auto prev_lock = &locks_[current->prev_];

        switch (prev_lock->type_)
        {
        case lock_type::xlock_keep:
        case lock_type::xlock_solo:
        {
          prev_lock->lock_->lock();
          prev_lock->called_ = true;
          prev_lock->locked_ = true;
        }
        break;
        case lock_type::slock_keep:
        case lock_type::slock_solo:
        {
          assert(!"unrechable state since same lock is re-entered");
        }
        break;
        }
      }
    }
    current->locked_ = false;
    current->called_ = false;
    current->invalid_ = true;

    --current_;
  }

  // xlock_solo 모드로 락을 잡고, 락 인덱스를 돌려줌 
  int8_t enter_xlock_solo(lockable* lock)
  {
    lock_info* e_lock = nullptr;
    int8_t prev = -1;

    // 이전 락들을 모두 역순으로 unlock. 
    for (int idx = current_; idx >= 0; --idx)
    {
      auto plock = &locks_[idx];
      
      if (plock->locked_ && plock->called_)
      {
        // called_ 상태 유지. exit할 때 처리에 사용

        switch (plock->type_)
        {
        case lock_type::xlock_keep:
        case lock_type::xlock_solo:
        {
          plock->lock_->unlock();
        }
        break;
        case lock_type::slock_keep:
        case lock_type::slock_solo:
        {
          plock->lock_->unlock_shared();
        }
        break;
        }
      }

      if (prev < 0 && plock->lock_ == lock)
      {
        e_lock = plock;
        prev = idx;
      }

      plock->locked_ = false; // all unlocked
    }

    if ((current_ + 1) >= max_lock_depth)
    {
      // throw exception
      return -1;
    }


    int8_t idx = current_ + 1;

    lock_info* n_lock = &locks_[idx];
    n_lock->lock_ = lock;
    n_lock->type_ = lock_type::xlock_solo;
    n_lock->locked_ = true;
    n_lock->called_ = true;
    n_lock->prev_ = prev;
    n_lock->next_ = -1;

    if (e_lock != nullptr)
    {
      e_lock->next_ = idx;
    }

    n_lock->lock_->lock();


    // lock is in xlock state 

    ++current_;
    n_lock->invalid_ = false;

    return current_;
  }

  void exit_xlock_solo(lockable* lock)
  {
    assert(current_ >= 0);

    auto current = &locks_[current_];

    assert(current != nullptr);
    assert(current->lock_ == lock);
    assert(current->type_ == lock_type::xlock_solo);
    assert(current->locked_);
    assert(current->invalid_ == false);

    assert(current->called_);

    current->lock_->unlock();
    current->locked_ = false;
    current->called_ = false;
    current->invalid_ = true;

    // 이전 락들을 모두 처음부터 차례로 락. 
    for (int idx = 0; idx < current_; ++idx)
    {
      auto plock = &locks_[idx];

      // todo: throw if lock type is invalid

      switch (plock->type_)
      {
      case lock_type::xlock_keep:
      case lock_type::xlock_solo:
      {
        if (plock->called_ && !plock->locked_)
        {
          plock->lock_->lock();
          plock->locked_ = true;
          set_locked_for_tail(plock);
        }
      }
      break;
      case lock_type::slock_keep:
      case lock_type::slock_solo:
      {
        if (plock->called_ && !plock->locked_)
        {
          plock->lock_->lock_shared();
          plock->locked_ = true;
          set_locked_for_tail(plock);
        }
      }
      break;
      }
    }

    --current_;
  }

  // slock_solo 모드로 락을 잡고, 락 인덱스를 돌려줌 
  int8_t enter_slock_solo(lockable* lock)
  {
    lock_info* e_lock = nullptr;
    int8_t prev = -1;

    // 이전 락들을 모두 역순으로 unlock. 
    for (int idx = current_; idx >= 0; --idx)
    {
      auto plock = &locks_[idx];

      if (plock->locked_ && plock->called_)
      {
        // called_ 상태 유지. exit할 때 처리에 사용

        switch (plock->type_)
        {
        case lock_type::xlock_keep:
        case lock_type::xlock_solo:
        {
          plock->lock_->unlock();
        }
        break;
        case lock_type::slock_keep:
        case lock_type::slock_solo:
        {
          plock->lock_->unlock_shared();
        }
        break;
        }
      }

      if (prev < 0 && plock->lock_ == lock)
      {
        e_lock = plock;
        prev = idx;
      }

      plock->locked_ = false; // all unlocked
    }

    if ((current_ + 1) >= max_lock_depth)
    {
      // throw exception
      return -1;
    }


    int8_t idx = current_ + 1;

    lock_info* n_lock = &locks_[idx];
    n_lock->lock_ = lock;
    n_lock->type_ = lock_type::slock_solo;
    n_lock->locked_ = true;
    n_lock->called_ = true;
    n_lock->prev_ = prev;
    n_lock->next_ = -1;

    if (e_lock != nullptr)
    {
      e_lock->next_ = idx;
    }

    n_lock->lock_->lock_shared();

    // lock is in slock state 

    ++current_;
    n_lock->invalid_ = false;

    return current_;
  }

  void exit_slock_solo(lockable* lock)
  {
    assert(current_ >= 0);

    auto current = &locks_[current_];

    assert(current != nullptr);
    assert(current->lock_ == lock);
    assert(current->type_ == lock_type::slock_solo);
    assert(current->locked_);
    assert(current->invalid_ == false);

    assert(current->called_);

    current->lock_->unlock_shared();
    current->locked_ = false;
    current->called_ = false;
    current->invalid_ = true;

    // 이전 락들을 모두 처음부터 차례로 락. 
    for (int idx = 0; idx < current_; ++idx)
    {
      auto plock = &locks_[idx];

      switch (plock->type_)
      {
      case lock_type::xlock_keep:
      case lock_type::xlock_solo:
      {
        if (plock->called_ && !plock->locked_)
        {
          plock->lock_->lock();
          plock->locked_ = true;
          set_locked_for_tail(plock);
        }
      }
      break;
      case lock_type::slock_keep:
      case lock_type::slock_solo:
      {
        if (plock->called_ && !plock->locked_)
        {
          plock->lock_->lock_shared();
          plock->locked_ = true;
          set_locked_for_tail(plock);
        }
      }
      break;
      }
    }

    --current_;
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

private: 
  enum class lock_type : uint8_t
  {
    xlock_solo, 
    slock_solo, 
    xlock_keep, 
    slock_keep,
  };

  // lock_info. note: keep the size small to keep the whole data in cache
  struct lock_info
  {
    lockable* lock_;
    int8_t  prev_;
    int8_t  next_;
    lock_type type_ : 2;
    bool locked_    : 1;
    bool called_    : 1;
    bool invalid_   : 1;

    lock_info()
      : lock_(nullptr)
      , prev_(-1)
      , type_()
      , locked_(false)
      , called_(false)
      , invalid_(true)
    {
    }
  };

private: 

  void set_locked_for_tail(lock_info* lock)
  {
    int8_t next = lock->next_;

    while (next >= 0)
    {
      assert(next < max_lock_depth);

      locks_[next].locked_ = true;
      next = locks_[next].next_;
    }
  }

private: 
  std::array<lock_info, max_lock_depth> locks_;       // used array for cache coherence
  int current_;
};

} // learn



