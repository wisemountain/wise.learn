#pragma once 

#include <shared_mutex>
#include <string_view>

namespace learn
{

class lockable : public std::shared_mutex
{
public:
  lockable(const char* name)
    : std::shared_mutex()
    , name_(name)
    , mode_latch_(false)
  {}

  const std::string_view& name() const
  {
    return name_;
  }

  void lock() noexcept
  {
    while (mode_latch_)
      ;

    std::shared_mutex::lock();
  }

  void unlock() noexcept 
  {
    std::shared_mutex::unlock();
  }

  void lock_shared() noexcept 
  {
    while (mode_latch_)
      ;

    std::shared_mutex::lock_shared();
  }

  void unlock_shared() noexcept 
  {
    std::shared_mutex::unlock_shared();
  }

  void upgrade() noexcept
  {
    mode_latch_ = 1;
    std::shared_mutex::unlock_shared();
    std::shared_mutex::lock();
    mode_latch_ = 0;
  }

  void downgrade() noexcept
  {
    mode_latch_ = 1;
    std::shared_mutex::unlock();
    std::shared_mutex::lock_shared();
    mode_latch_ = 0;
  }

private: 
  std::string_view name_;
  std::atomic<bool> mode_latch_;
};

} // learn