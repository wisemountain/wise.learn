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

class lockable : protected std::shared_mutex
{
public: 
  static constexpr int retry_sleep_count = 1000;

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
      wait_latch();

      std::shared_mutex::lock(); 

      bv = mode_latch_;

      if (bv > 0) // lock ȣ�� ���ȿ� ��ġ�� �����ִٸ� 
      {
        // ��ġ�� ���� ������ ���� ����
        std::shared_mutex::unlock();
      }
      else
      {
        break; // ���� �ٽ� ��ȸ�ϱ� ���� ���� ������ ó���Ǿ�� ��
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
      wait_latch();

      std::shared_mutex::lock_shared();

      bv = mode_latch_;

      if (bv > 0)
      {
        // ��ġ�� ���� ������ ���� ����
        std::shared_mutex::unlock_shared();
      }
      else
      {
        break; // ���� �ٽ� ��ȸ�ϱ� ���� ���� ������ ó���Ǿ�� ��
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
    std::shared_mutex::lock();              // ���⼭ ����ϴ� ������ upgrade �� ���� �ذ�

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

  void wait_latch()
  {
      int try_count = 0;

      while (mode_latch_)
      {
        cpu_relax();

        ++try_count;

        if (try_count >= retry_sleep_count)
        {
          sleep(1);
          try_count = 0;
        }
      }
  }

  void sleep(int ms)
  {
    std::this_thread::sleep_for(std::chrono::milliseconds(ms));
  }

private: 
  std::string_view name_;
  std::atomic<int> mode_latch_;
};

} // learn