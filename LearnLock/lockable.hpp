#pragma once 

#include <cassert>
#include <shared_mutex>
#include <string_view>

#ifdef _MSC_VER
#define WINDOWS_LEAN_AND_MEAN 
#include <windows.h>
#define cpu_relax() YieldProcessor()
#else
#define cpu_relax() { \
   static constexpr std::chrono::microseconds us0{ 0 }; \
   std::this_thread::sleep_for( us0);  \
}
#endif

namespace learn
{

class lockable : public std::shared_mutex
{
public:
  lockable(const char* name)
    : std::shared_mutex()
    , name_(name)
    , mode_latch_(0)
  {}

  const std::string_view& name() const
  {
    return name_;
  }

  void lock() noexcept
  {
    int bv = 0;

    do
    {
      while (mode_latch_)
      {
        cpu_relax();
      }

      std::shared_mutex::lock(); 

      bv = mode_latch_;

      if (bv > 0) // lock 호출 동안에 래치가 잡혀있다면 
      {
        // 래치가 잡혀 있으면 나는 포기
        std::shared_mutex::unlock();
      }
      else
      {
        break; // 값을 다시 조회하기 전에 같은 값으로 처리되어야 함
      }
    } while (bv > 0);
  }

  void unlock() noexcept 
  {
    std::shared_mutex::unlock();
  }

  void lock_shared() noexcept 
  {
    int bv = 0;

    do
    {
      while (mode_latch_)
      {
        cpu_relax();
      }

      std::shared_mutex::lock_shared();

      bv = mode_latch_;

      if (bv > 0)
      {
        // 래치가 잡혀 있으면 나는 포기
        std::shared_mutex::unlock_shared();
      }
      else
      {
        break; // 값을 다시 조회하기 전에 같은 값으로 처리되어야 함
      }
    } while (bv > 0);
  }

  void unlock_shared() noexcept 
  {
    std::shared_mutex::unlock_shared();
  }

  void upgrade() noexcept
  {
    assert(mode_latch_ >= 0);

    ++mode_latch_;                          

    std::shared_mutex::unlock_shared();
    std::shared_mutex::lock();              // 여기서 대기하는 것으로 upgrade 간 경쟁 해결

    --mode_latch_;
  }

  void downgrade() noexcept
  {
    assert(mode_latch_ >= 0);

    ++mode_latch_;

    std::shared_mutex::unlock();
    std::shared_mutex::lock_shared();

    --mode_latch_;
  }

private: 
  std::string_view name_;
  std::atomic<int> mode_latch_;
};

} // learn