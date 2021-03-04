#include "pch.h"

#include "CallbackSample.h"

#using <mscorlib.dll>

using namespace System;
using namespace System::Collections::Generic;

void testTypes()
{
    List<int>^ lst = gcnew List<int>();

    lst->Add(1);
    lst->Add(2);

    List<String^>^ slst = gcnew List<String^>();

    slst->Add(gcnew String("Hello"));
    slst->Add(gcnew String("World"));
}

int main(array<System::String^>^ args)
{
    MyClass^ mc = gcnew MyClass();

    // create a thunk and link it to the managed class

    EnumWindowsProcThunk t(mc);

    // Call Window's EnumWindows() C API with the pointer
    // to the callback and our thunk as context parameter

    ::EnumWindows(&EnumWindowsProcThunk::fwd, (LPARAM)&t);

    return 0;
}
