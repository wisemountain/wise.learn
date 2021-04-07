
set(VALUE_1 "Hello CMake")

function(SIMPLE REQUIRED_ARG)
	message("Function name is SIMPLE")
	message(STATUS "Simple arguments: ${REQUIRED_ARG}, followed by ${ARGV}")
	set(${REQUIRED_ARG} "From SIMPLE" PARENT_SCOPE)
endfunction()

SIMPLE(This)

function(COMPLEX)
	cmake_parse_arguments(
		COMPLEX_PREFIX
		"SINGLE;"ANOTHER"
		"ONE_VALUE;AlSO_ONE_VALUE"
		"MULTI_VALUES"
		${ARGN}
	)

	message("SINGLE ${COMPLEX_PREFIX_SINGLE}")
	message("ANOTHER ${COMPLEX_PREFIX_ANOTHER}")
	message("ONE_VALUE ${COMPLEX_PREFIX_ONE_VALUE}")
	message("MULTI_VALUES ${COMPLEX_PREFIX_MULTI_VALUES}")
endfunction()


message("Output: ${VALUE_1}")

comple(SINGLE ONE_VALUE value MULTI_VALUES some other values)
