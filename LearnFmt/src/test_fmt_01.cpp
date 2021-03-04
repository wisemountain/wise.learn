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
// spdlog�� ����� fmt�� ����� ��� spdlog.h�� ���� 
// ���Խ��Ѿ� FMT_HEADER_ONLY ��ũ�θ� ���� ���´�. 
// ���� ������ ��� �����ϱ� ���� �� ��ũ�θ� Ȱ��ȭ�ؾ� �Ѵ�.
//