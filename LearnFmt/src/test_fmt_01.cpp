#include <pch.hpp>
#include <catch.hpp>
#include <spdlog/spdlog.h>


struct date {
	int year, month, day;
};

template <>
struct fmt::formatter<date> {
	constexpr auto parse(format_parse_context& ctx) { return ctx.begin(); }

	template <typename FormatContext>
	auto format(const date& d, FormatContext& ctx) {
		return format_to(ctx.out(), "{}-{}-{}", d.year, d.month, d.day);
	}
};


TEST_CASE("fmt", "learnfmt")
{

	SECTION("usage. custom formatter")
	{
		std::string s = fmt::format("The date is {}", date{ 2012, 12, 9 });

		REQUIRE(s == "The date is 2012-12-9");
	}

	SECTION("options")
	{

	}
}

//
// spdlog에 번들된 fmt를 사용할 경우 spdlog.h를 통해 
// 포함시켜야 FMT_HEADER_ONLY 매크로를 통해 들어온다. 
// 직접 포함할 경우 포함하기 전에 이 매크로를 활성화해야 한다.
//