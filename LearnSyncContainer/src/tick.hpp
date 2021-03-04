#pragma once

#include <chrono>

namespace util {

using tick_t = uint64_t;

template <typename CLOCK>
class tick
{
	using clock = CLOCK;

public:
	tick()
	{
		start_ = clock::now();
	}

	tick_t elapsed() const
	{
		auto lap = std::chrono::duration_cast<std::chrono::milliseconds>(clock::now() - start_);

		return lap.count(); // milliseconds
	}

	tick_t since_epoch() const
	{
		auto now = std::chrono::high_resolution_clock::now().time_since_epoch();
		return std::chrono::duration_cast<std::chrono::nanoseconds>(now).count();
	}

	bool check_timeout(tick_t time, bool reset_on_timeout = true)
	{
		if (elapsed() > time)
		{
			if (reset_on_timeout)
			{
				reset();
			}

			return true;
		}

		return false;
	}

	void reset()
	{
		start_ = clock::now();
	}

private:
	typename clock::time_point start_;
};

using simple_tick = tick<std::chrono::system_clock>;
using fine_tick = tick<std::chrono::steady_clock>;

} // util
