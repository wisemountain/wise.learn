#pragma once 

#include <cassert>
#include <shared_mutex>
#include <string_view>

namespace learn
{

class lockable : protected std::shared_mutex
{
public:
  lockable(const char* name)
    : std::shared_mutex()
    , name_(name)
  {}

  const std::string_view& name() const
  {
    return name_;
  }

  void lock() noexcept
  {
    std::shared_mutex::lock();
  }

  void unlock() noexcept 
  {
    std::shared_mutex::unlock();
  }

  void lock_shared() noexcept
  {
    std::shared_mutex::lock_shared();
  }

  void unlock_shared() noexcept 
  {
    std::shared_mutex::unlock_shared();
  }

  void upgrade() noexcept
  {
    std::shared_mutex::unlock_shared();
    std::shared_mutex::lock();              
  }

  void downgrade() noexcept
  {
    std::shared_mutex::unlock();
    std::shared_mutex::lock_shared();
  }

private: 
  std::string_view name_;
};

} // learn