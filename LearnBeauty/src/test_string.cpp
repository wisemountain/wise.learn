#include <pch.hpp>
#include <catch.hpp>
#include <string>

TEST_CASE("string", "beauty")
{
	SECTION("interface")
	{
		auto s1 = std::string("hello"); // operator = 

		std::string s2;
		s2.assign("hello"); // assign

		std::string s3{ "Hello" };
		std::string s4{ 'H', 'e', 'l', 'l', 'o' };
		std::string s5 = { "Hello" };

		s5 += { "world!"};
		s5.assign({ "Hello world!" });

		s4.append("world!");

		SECTION("iterator w/ initializer_list")
		{
			// iterator insert(const const_iterator _Where, const initializer_list<_Elem> _Ilist) 
			// - 위 코드 사용하기. 
			// - xstring@2606

			std::initializer_list<char> world = { 'w', 'o', 'r', 'l', 'd' };
			s3.insert(s3.cbegin() + 5, world);
		}
	}

	SECTION("methods")
	{
		// xstring (visual c++)
		// 
	}
}

TEST_CASE("c++11+", "methods")
{
	SECTION("initializer_list")
	{
		std::initializer_list<char> world = { 'w', 'o', 'r', 'l', 'd' };

		std::string s3("Hello ");
		s3.insert(s3.cbegin() + 5, world);
	}

}