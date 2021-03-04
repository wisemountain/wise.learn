using namespace System;

#include <vcclr.h>

// Managed class with the desired delegate

public ref class MyClass
{
public:
    delegate bool delOnEnum(int h);
    event delOnEnum^ OnEnum;

    MyClass()
    {
        OnEnum += gcnew delOnEnum(this, &MyClass::handler);
    }

    bool handler(int h)
    {
        System::Console::WriteLine("Found a new window {0}", h);
        return true;
    }
    
    bool fire(int h)
    {
        return OnEnum(h);
    }
};

class EnumWindowsProcThunk
{
private:
    // hold reference to the managed class

    msclr::gcroot<MyClass^> m_clr;
public:

    // the native callback

    static BOOL CALLBACK fwd(
        HWND hwnd,
        LPARAM lParam)
    {
        // cast the lParam into the Thunk (native) class,
        // then get is managed class reference,
        // finally call the managed delegate

        return static_cast<EnumWindowsProcThunk*>(
            (void*)lParam)->m_clr->fire((int)hwnd) ? TRUE : FALSE;
    }

    // Constructor of native class that takes a reference to the managed class

    EnumWindowsProcThunk(MyClass^ clr)
    {
        m_clr = clr;
    }
};
