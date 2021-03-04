#include <pch.hpp>
#include <catch.hpp>

#pragma warning(disable: 6319)

int f()
{
	return 3;
}

int fact(int n)
{
	if (n == 0)
	{
		return 1;
	}

	return n * fact(n - 1);
}

unsigned fac_tailrec(unsigned acc, unsigned n)
{
	if (n < 2) return acc;
	return fac_tailrec(n * acc, n - 1);
}

unsigned fac(unsigned n)
{
	return fac_tailrec(1, n);
}


TEST_CASE("assembly", "beauty")
{
	SECTION("simplest")
	{
		auto v = f();

		CHECK(v == 3);
	}

	SECTION("string")
	{
		std::string s("hello");

		s += "world";
	}

	SECTION("factorial")
	{
		auto f = fact(3);
			
		CHECK(f == 3 * 2 * 1);
	}

	SECTION("factorial tali recursion")
	{
		auto f = fac(10);

		CHECK(f == 10*9*8*7*6*5*4*3*2*1);
	}
}