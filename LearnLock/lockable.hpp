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
    , mode_latch_(false)
  {}

  const std::string_view& name() const
  {
    return name_;
  }

  void lock() noexcept
  {
    bool bv = false;

    do
    {
      while (mode_latch_.load(std::memory_order::memory_order_acquire))
        cpu_relax();

      std::shared_mutex::lock();

      bv = mode_latch_.load(std::memory_order::memory_order_acquire);

      if (bv) // lock ȣ�� ���ȿ� ��ġ�� �����ִٸ� 
      {
        // ��ġ�� ���� ������ ���� ����
        std::shared_mutex::unlock();
      }
      else
      {
        break; // ���� �ٽ� ��ȸ�ϱ� ���� ���� ������ ó���Ǿ�� ��
      }
    } while (bv);
  }

  void unlock() noexcept 
  {
    std::shared_mutex::unlock();
  }

  void lock_shared() noexcept 
  {
    bool bv = false;

    do
    {
      while (mode_latch_.load(std::memory_order::memory_order_acquire))
        cpu_relax();

      std::shared_mutex::lock_shared();

      bv = mode_latch_.load(std::memory_order::memory_order_acquire);

      if (bv)
      {
        // ��ġ�� ���� ������ ���� ����
        std::shared_mutex::unlock_shared();
      }
      else
      {
        break; // ���� �ٽ� ��ȸ�ϱ� ���� ���� ������ ó���Ǿ�� ��
      }
    } while (bv);
  }

  void unlock_shared() noexcept 
  {
    std::shared_mutex::unlock_shared();
  }

  void upgrade() noexcept
  {
    bool bv = false;
    bool unlocked = false;

    while (!mode_latch_.compare_exchange_strong(bv, true))
    {
      cpu_relax();
      bv = false;

      if (!unlocked)
      {
        // ��ġ�� ���� ������ ���� Ǯ���ش�.
        std::shared_mutex::unlock_shared();
        unlocked = true;
      }
    }

    assert(bv == false);
    assert(mode_latch_);

    if (!unlocked)
    {
      std::shared_mutex::unlock_shared();
    }

    std::shared_mutex::lock();
    mode_latch_.store(false, std::memory_order::memory_order_release);
  }

  void downgrade() noexcept
  {
    bool bv = false;
    bool unlocked = false;

    while (!mode_latch_.compare_exchange_strong(bv, true))
    {
      cpu_relax();
      bv = false;

      if (!unlocked)
      {
        // ��ġ�� ���� ������ ���� Ǯ���ش�.
        std::shared_mutex::unlock();
        unlocked = true;
      }
    }

    assert(bv == false);
    assert(mode_latch_);

    if (!unlocked)
    {
      std::shared_mutex::unlock();
    }

    std::shared_mutex::lock_shared();
    mode_latch_.store(false, std::memory_order::memory_order_release);
  }

private: 
  std::string_view name_;
  std::atomic<bool> mode_latch_;
};

} // learn