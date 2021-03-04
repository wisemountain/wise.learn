#include <pch.hpp>
#include <catch.hpp>

struct hello
{
	int v;
};

template <typename T, typename U>
auto add(T t, U u ) -> decltype(t + u) {
	return t + u;
}

TEST_CASE("decltype", "learnfmt")
{
	SECTION("simple")
	{
		hello h;
		using ht = decltype(h); // decltype works on expresson (not declaration, nor statement)
		h.v = 0;

		REQUIRE(typeid(ht) == typeid(hello));
	}

	SECTION("usage1")
	{
		auto v = add(3.0, 1);
		REQUIRE(v == 4.0);
	}

	SECTION("usage2")
	{

	}
}