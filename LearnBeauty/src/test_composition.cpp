#include <pch.hpp>
#include <catch.hpp>
#include <functional>

template <typename T>
T add(T a, T b)
{
	return a + b;
}

template <typename T>
T mul(T a, T b)
{
	return a * b;
}

template <typename T>
std::function<T(T, T)> compose(std::function<T(T, T)> f, std::function< T(T, T)> g)
{
	return [=](T a, T b) -> T
	{
		return f(g(a, b), g(a, b));
	};
}

template <typename T>
T ma(T a, T b)
{
	return mul(add(a, b), add(a, b));
}

TEST_CASE("composition", "learnbeauty")
{
	SECTION("lambda")
	{	
		auto f1 = [](auto a, auto b) { return add(a, b); };
		auto g1 = [](auto a, auto b) { return mul(a, b); };

		auto c = compose<float>(f1, g1);

		// in c++, we use composition always. not in haskell way, but in c++ way.
	}

	SECTION("c way")
	{
		auto c = ma(3, 5);
		CHECK(c == (3 + 5) * (3 + 5)); 
	}

	SECTION("string")
	{
		auto r = std::string("Hello") + " " + std::string("world!");
		CHECK(r == "Hello world!");
	}
}