#include "../doctest.h"

#include <string>
#include <iostream>
#include <vector>

namespace
{
struct Entry
{
	std::string name;
	int number;
};

std::ostream& operator<<(std::ostream& os, const Entry& e)
{
	return os << "{" << e.name << ", " << e.number << "}";
}
} // anonymous

TEST_CASE("ss chap 4")
{
	SUBCASE("4.3.3 I/O of user defined types")
	{	
		Entry e{ "Hello", 3 };

		std::cout << e << std::endl;
	}

	SUBCASE("4.4.1 vector")
	{
		std::vector<Entry> phone_book = {
			{ "David Hume", 123456},
			{ "Karl Popper", 234567}
		};

		CHECK(phone_book.size() == 2);
	}
}