#include <pch.hpp>
#define CATCH_CONFIG_COLOUR_WINDOWS
#define CATCH_CONFIG_RUNNER
#include <catch.hpp>

int main(int argc, char* argv[])
{
    Catch::Session session;

    int returnCode = session.applyCommandLine(argc, argv);

    if (returnCode != 0)
    {
        return returnCode;
    }

    int numFailed = session.run();
    return numFailed;
}