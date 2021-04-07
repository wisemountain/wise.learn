cmake_minimum_required(VERSION 2.4)

set(CMAKE_CXX_STANDARD 17)

project(hello_world 
		VERSION 1.0 
		DESCRIPTION "Hello world"
		LANGUAGES CXX)



# file(GLOB SOURCE_FILES dir/*.hpp dir/*.cpp)

include_directories(${PROJECT_SOURCE_DIR})
add_executable(app main.cpp foo.cpp)

# using clang
# -DCMAKE_C_COMPILER=/usr/bin/clang -DCMAKE_CXX_COMPILER=/usr/bin/clang
#

# https://cliutils.gitlab.io/modern-cmake/chapters/basics.html
# - good for basics


