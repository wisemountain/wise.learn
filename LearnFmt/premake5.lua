local SPDLOG_HOME = os.getenv("SPDLOG_HOME")
local CATCH_HOME = os.getenv("CATCH_HOME")
local CATCH_INCLUDE_DIR = CATCH_HOME .. "/single_include/catch2"

workspace "LearnFmt"
	location "generated"
	language "C++"
	architecture "x86_64"
	
	configurations { "Debug", "Release" }
	
	filter { "configurations:Debug" }
		symbols "On"
	
	filter { "configurations:Release" }
		optimize "On"
	
	filter { }

	
	targetdir ("build/bin/%{prj.name}/%{cfg.longname}")
    objdir ("build/obj/%{prj.name}/%{cfg.longname}")
	
function includeSPDLOG()
	includedirs (SPDLOG_HOME .. "/include")
end	

function includeCatch()
	includedirs( CATCH_INCLUDE_DIR )
	defines "CATCH_CPP11_OR_GREATER"
end


project "LearnFmt"
	kind "ConsoleApp"

	files "src/**"
	vpaths { ["src"] = { "src/**"} }

	pchheader "pch.hpp"
	pchsource "src/pch.cpp"

	warnings "extra"
	buildoptions { "/std:c++17" }

	includedirs "src/"
	includeCatch()
	includeSPDLOG()

	staticruntime "On"

