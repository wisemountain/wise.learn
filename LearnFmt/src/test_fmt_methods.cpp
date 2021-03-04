#include <pch.hpp>
#include <catch.hpp>
#include <spdlog/spdlog.h>
#include <iostream>


template <typename T>
void print(T arg) {
	std::cout << arg << std::endl;
}

template <typename T, typename... Types>
void print(T arg, Types... args) {
	std::cout << arg << ", ";
	print(args...);
}

template <typename... Ints>
int sum_all(Ints... nums) {
	return (... + nums);
}

TEST_CASE("fmt method", "learnfmt")
{

	SECTION("debug")
	{
		std::string s = fmt::format("Number: {}", 3);

		// 대단한 사람들이 많다. 
		// - string_view는 유틸. c++17부터 표준
		// - vformat, format_to와 같이 진행된다. 
		// - arg를 저장한 곳에 타잎 추론으로 몇 가지 정보를 포함한다. 
		// - 나머지는 문자열 파싱과 값 교체이다. 
		// - format_arg_store에 값들 저장
		//   - 여기가 MPL의 핵심이다. 
		// fmt는 std::format()으로 c++20에서 표준화 절차를 밟고 있다. 
	}

	SECTION("method1. variadic template")
	{
		print(1, 2, 3);

		// print(1, ...args)
		// print(2, ...args)
		// print(3)
		// 재귀와 base case
	}

	SECTION("method2. folding")
	{
		// 
		// 1 + 4 + 2 + 3 + 10
		std::cout << sum_all(1, 4, 2, 3, 10) << std::endl;
	}


}
