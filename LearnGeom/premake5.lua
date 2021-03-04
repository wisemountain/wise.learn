local BOOST_HOME = os.getenv("BOOST_HOME")
local GLFW_HOME = os.getenv("GLFW_HOME")
local GLEW_HOME = os.getenv("GLEW_HOME")
local GLM_HOME = os.getenv("GLM_HOME")
local STB_HOME = os.getenv("STB_HOME")
local ASSIMP_HOME = os.getenv("ASSIMP_HOME")

workspace "LearnGeom"
	-- We set the location of the files Premake will generate
	location "generated"
	
	-- We indicate that all the projects are C++ only
	language "C++"
	
	-- We will compile for x86_64. You can change this to x86 for 32 bit builds.
	architecture "x86_64"
	
	-- Configurations are often used to store some compiler / linker settings together.
    -- The Debug configuration will be used by us while debugging.
    -- The optimized Release configuration will be used when shipping the app.
	configurations { "Debug", "Release" }
	
	-- We use filters to set options, a new feature of Premake5.
	
	-- We now only set settings for the Debug configuration
	filter { "configurations:Debug" }
		-- We want debug symbols in our debug config
		symbols "On"
	
	-- We now only set settings for Release
	filter { "configurations:Release" }
		-- Release should be optimized
		optimize "On"
	
	-- Reset the filter for other settings
	filter { }
	
	-- Here we use some "tokens" (the things between %{ ... }). They will be replaced by Premake
	-- automatically when configuring the projects.
	-- * %{prj.name} will be replaced by "ExampleLib" / "App" / "UnitTests"
	--  * %{cfg.longname} will be replaced by "Debug" or "Release" depending on the configuration
	-- The path is relative to *this* folder
	targetdir ("build/bin/%{prj.name}/%{cfg.longname}")
    objdir ("build/obj/%{prj.name}/%{cfg.longname}")
	
function useBOOST()
	includedirs (BOOST_HOME .. "/include")
end	

function useGLFW()
	includedirs (GLFW_HOME .. "/include")
	libdirs (GLFW_HOME .. "/lib-vc2019")
	
	links "glfw3"
end

function useGLEW()
	includedirs (GLEW_HOME .. "/include")
	libdirs (GLEW_HOME .. "/lib/Release/x64")
	
	links "glew32"
end

function useGLM()
	includedirs (GLM_HOME .. "/glm")
end

function useSTB()
	includedirs (STB_HOME)
end

function useASSIMP()
	includedirs (ASSIMP_HOME .. "/include")

	libdirs (ASSIMP_HOME .. "/lib")
	
	links "assimp-vc140-mt"
end

project "LearnGeom"
	kind "ConsoleApp"

	includedirs ("src")
	includedirs ("src/wykobi")
	includedirs ("src/imgui")
	
	files { "src/**" }

	-- filter 생성을 위해 필요.
	vpaths { 
		["wykobi"] = "src/wykobi/**",
		["src"] = "src/*.cpp"
	}
	
	links "opengl32"
	links "glu32"

	useBOOST()
	useGLFW()
	useGLEW()
	
    
