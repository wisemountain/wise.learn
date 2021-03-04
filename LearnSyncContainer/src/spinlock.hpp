
//          Copyright Oliver Kowalke 2016.
// Distributed under the Boost Software License, Version 1.0.
//    (See accompanying file LICENSE_1_0.txt or copy at
//          http://www.boost.org/LICENSE_1_0.txt)

// Modified and adapted for wise from spinlock_ttas_adaptive.hpp

#pragma once

#include <algorithm>
#include <atomic>
#include <chrono>
#include <cmath>
#include <random>
#include <thread>

// spinlock
#define FIBERS_CONTENTION_WINDOW_THRESHOLD 16
#define FIBERS_RETRY_THRESHOLD 64
#define FIBERS_SPIN_BEFORE_SLEEP0 256
#define FIBERS_SPIN_BEFORE_YIELD 128



// based on informations from:
// https://software.intel.com/en-us/articles/benefitting-power-and-performance-sleep-loops
// https://software.intel.com/en-us/articles/long-duration-spin-wait-loops-on-hyper-threading-technology-enabled-intel-processors

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

namespace util {

enum class spinlock_status
{
	locked = 0,
	unlocked
};

/// spinlock from boost::fiber 
/**
 * look at spinlock_ttas.hpp at boost for detailed explanantion
 */
class spinlock
{
private:
	std::atomic< spinlock_status >              state_{ spinlock_status::unlocked };
	std::atomic< std::size_t >                  retries_{ 0 };

public:
	spinlock() = default;
	spinlock(spinlock const&) = delete;
	spinlock& operator=(spinlock const&) = delete;

	void lock() noexcept
	{
		static thread_local std::minstd_rand generator{ std::random_device{}() };

		std::size_t collisions = 0;

		for (;;)
		{
			std::size_t retries = 0;
			const std::size_t prev_retries = retries_.load(std::memory_order_relaxed);

			const std::size_t max_relax_retries = (std::min)(
				static_cast<std::size_t>(FIBERS_SPIN_BEFORE_SLEEP0), 2 * prev_retries + 10);

			const std::size_t max_sleep_retries = (std::min)(
				static_cast<std::size_t>(FIBERS_SPIN_BEFORE_YIELD), 2 * prev_retries + 10);

			while (spinlock_status::locked == state_.load(std::memory_order_relaxed))
			{
				if (max_relax_retries > retries)
				{
					++retries;
					cpu_relax();
				}
				else if (max_sleep_retries > retries)
				{
					++retries;
					static constexpr std::chrono::microseconds us0{ 0 };
					std::this_thread::sleep_for(us0);
				}
				else
				{
					std::this_thread::yield();
				}
			}

			if (spinlock_status::locked == state_.exchange(spinlock_status::locked, std::memory_order_acquire))
			{
				std::uniform_int_distribution< std::size_t > distribution{
					0,
					static_cast<std::size_t>(1) <<
					(std::min)(collisions, static_cast<std::size_t>(FIBERS_CONTENTION_WINDOW_THRESHOLD))
				};

				const std::size_t z = distribution(generator);

				++collisions;

				for (std::size_t i = 0; i < z; ++i)
				{
					cpu_relax();
				}
			}
			else
			{
				retries_.store(prev_retries + (retries - prev_retries) / 8, std::memory_order_relaxed);
				break;
			}
		}
	}

	bool try_lock() noexcept
	{
		return spinlock_status::unlocked == state_.exchange(spinlock_status::locked, std::memory_order_acquire);
	}

	void unlock() noexcept
	{
		state_.store(spinlock_status::unlocked, std::memory_order_release);
	}
};

} // util
