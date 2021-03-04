#include <pch.hpp>
#include <catch.hpp>

#include "spinlock.hpp"
#include "tick.hpp"

#include <atomic>
#include <chrono>
#include <concurrent_vector.h>
#include <vector>
#include <mutex>
#include <shared_mutex>
#include <thread>
#include <unordered_map>


namespace {

struct pc
{
	int id = 0;
	int sector_id = 0;
	std::size_t index = 0;
	bool removed = false;

	pc(int _id)
		: id(_id)
	{
	}

};


class sector
{
	using pc_map = std::unordered_map<int, pc*>;
	using vec = std::vector<pc*>;

public:

	void insert(pc* _pc)
	{
		// pc locked 
		{
			std::lock_guard<util::spinlock> lock(lock_pc_);
			pcs_.insert(pc_map::value_type(_pc->id, _pc));
			_pc->sector_id = get_sector_id();
		}

		// sync locked
		{
			std::unique_lock ul(lock_sync_);
			cv_.push_back(_pc);
		}
	}

	void remove(pc* _pc)
	{
		// pc locked
		{
			std::lock_guard<util::spinlock> lock(lock_pc_);
			_pc->sector_id = 0; 
			pcs_.erase(_pc->id);
		}

		// purge
		{
			std::unique_lock ul(lock_sync_);

			cv_.erase(std::find_if(cv_.begin(), cv_.end(),
				[this](pc* _pc) {
					return _pc->sector_id != get_sector_id();
				}), cv_.end());
		}
	}

	void copy_sync_list(vec& dst)
	{
		std::shared_lock sl(lock_sync_);
		dst.assign(cv_.begin(), cv_.end()); // 전체 복사. copy가 더 빠른가?
	}

	int get_sector_id() const
	{
		return sector_id_;
	}

private:
	int sector_id_ = 1;
	util::spinlock lock_pc_;
	std::shared_mutex lock_sync_; // read write lock (rwspinlock이라고 봐도 되고, 스핀락을 써도 된다)
	pc_map pcs_;
	vec cv_;
};

void sleep(std::size_t msec)
{
	std::this_thread::sleep_for(std::chrono::milliseconds(msec));
}

} // noname

TEST_CASE("concurrent vector", "sync")
{

	SECTION("v2")
	{
		sector s1;
		std::thread m;
		std::thread s;
		bool stop = false;

		for (int i = 0; i < 200; ++i)
		{
			s1.insert(new pc(i + 1));
		}

		std::thread([&s1, &stop]()
			{
				while (!stop) {
					pc* pc1 = new pc(500);
					s1.insert(pc1);
					s1.remove(pc1);
				}
			}).swap(m);

		std::vector<pc*> lst;

		std::thread([&s1, &stop, &lst]()
			{
				while (!stop)
				{
					lst.clear();
					s1.copy_sync_list(lst);

					for (auto& p : lst)
					{
						if (p->sector_id == s1.get_sector_id())
						{

						}
					}
				}
			}).swap(s);

		sleep(10000);

		stop = true;

		m.join();
		s.join();
	}
}
