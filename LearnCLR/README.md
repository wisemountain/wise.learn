# LearnCLR

## 읽기 1 

https://www.codeproject.com/Articles/19354/Quick-C-CLI-Learn-C-CLI-in-less-than-10-minutes

^ : a handle. a managed pointer 

```c++
System::Object^ x = gecnew System::Object(); 
```

% : a tracking reference. 

#using <mscorlib.dll>
using namespace System;


ref : a managed class

value : a value type. ie., struct.

public ref class 
public value struct
public enum class

cli::array<int>


```c++
using namespace System::Collections::Generic;

List<String^>^ slst = gcnew List<String^>();

slst->Add(gcnew String("Hello"));
slst->Add(gcnew String("World"));
```

컨테이너 사용도 자유롭다. 핸들만 잘 구분하면 된다. 


```c++
  property int X
    {
      int get() { return _x; }
      void set(int x) { _x = x; }
    }
  property String^ Name
  {
    String^ get() { return _name; }
    void set(String^ N) { _name = N; }
  }
```
속성은 함수 형식을 갖는다. 


## C 콜백 처리 

위의 읽기의 예시 중 하나이다. 

```c++
  msclr::auto_gcroot<MyClass^> m_clr;
```

```c++
#include <span class="code-keyword"><vcclr.h></span>
```

vcclr.h를 위와 같이 포함하여 auto_gcroot를 c++ 클래스에서 사용한다. 


```c++
using namespace System;

#include <span class="code-keyword"><vcclr.h></span>

// Managed class with the desired delegate

public ref class MyClass
{
public:
  delegate bool delOnEnum(int h);
  event delOnEnum ^OnEnum;

  bool handler(int h)
  {
    System::Console::WriteLine("Found a new window {0}", h);
    return true;
  }

  MyClass()
  {
    OnEnum = gcnew delOnEnum(this, &MyClass::handler);
  }
};

class EnumWindowsProcThunk
{
private:
  // hold reference to the managed class

  msclr::auto_gcroot<MyClass^> m_clr;
public:

  // the native callback

    static BOOL CALLBACK fwd(
    HWND hwnd,
    LPARAM lParam)
  {
      // cast the lParam into the Thunk (native) class,
      // then get is managed class reference,
      // finally call the managed delegate

      return static_cast<EnumWindowsProcThunk *>(
            (void *)lParam)->m_clr->OnEnum((int)hwnd) ? TRUE : FALSE;
  }

    // Constructor of native class that takes a reference to the managed class

  EnumWindowsProcThunk(MyClass ^clr)
  {
    m_clr = clr;
  }
};

int main(array<System::String ^> ^args)
{
  // our native class

  MyClass ^mc = gcnew MyClass();

    // create a thunk and link it to the managed class

  EnumWindowsProcThunk t(mc);

    // Call Window's EnumWindows() C API with the pointer
    // to the callback and our thunk as context parameter

  ::EnumWindows(&EnumWindowsProcThunk::fwd, (LPARAM)&t);

  return 0;
}
```

하나의 예시이다. 이건 그대로 따라해볼 필요가 있다. 

auto_gcroot는 제거되었다. gcroot로 처리. 

OnEnum의 raiser에 접근할 수 없다는 메세지가 나와 fire() 함수를 추가해서 간접적으로 호출했다. 

이와 같은 점들을 고려하고 공부를 더 하면서 구현한다. 


## native와 managed 클래스간의 호출

msclr::gcroot<Class^> 에서 ^로 지정해야 한다. 
한 두 시간 넘게 이것 때문에 헤맸다. 

다행이 auto_gcroot가 사라졌지만 샘플에 ^이 있어서 알았다. 


