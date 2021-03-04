# fmt 

## methods

```c++
template <typename S, typename... Args, typename Char = char_t<S>>
inline std::basic_string<Char> format(const S& format_str, Args&&... args) {
  return internal::vformat(
      to_string_view(format_str),
      {internal::make_args_checked<Args...>(format_str, args...)});
}
```

`char_t<S>`는 무엇?

```c++
/** String's character type. */
template <typename S> using char_t = typename internal::char_t_impl<S>::type;
```

`char_t_impl<S>`는 타잎 추론. void는 에러 나도록 함

```c++
template <typename S, typename = void> struct char_t_impl {};
template <typename S> struct char_t_impl<S, enable_if_t<is_string<S>::value>> {
  using result = decltype(to_string_view(std::declval<S>()));
  using type = typename result::char_type;
};
```

decltype과 std::declval을 사용한다. 
decltype은 auto의 타잎 추론과 비슷한 타잎 추론을 사용한다. 

https://modoocode.com/294
모두의 코드. c++에 대해 잘 정리해 두었다. 

먼저 따라가고 다시 온다. 


# decltype과 그 친구 std::declval

모두의 코드 따라가기.

## Value category

- xvalue, lvalue, prvalue

c++ 식은 타잎과 값 카테고리를 갖는다. 
c++에서 식의 값 카테고리는 따질 때 크게 두 가지를 따진다. 
- 정체를 알 수 있는가? 다른 식과 구분할 수 있는가?
- 이동 시킬 수 있는가? 
  - 이동 생성자, 이동 대입 연산자에서 사용할 수 있는가?


정체를 알 수 있다. 
이동 시킬 수 있다.

xvalue는 정체를 알 수 있고 이동 시킬 수 있다. 
prvalue는 정체를 알 수 없고 이동 시킬 수 있다. 
lvalue는 정체를 알 수 있고 이동 시킬 수 없다. 
정체도 모르고 이동도 못 하면 쓸 수가 없다. 

정체를 알 수 있는 모든 식을 glvalue라고 하고, 
이동 시킬 수 있는 모든 식을 rvalue라고 한다. 


xvalue : eXpiring value
prvalue : pure rvalue 
glvalue : generalized lvalue 


glvalue = lvalue \cup xvalue 
rvalue = prvalue \cup xvalue 

타잎과 값 분류는 다르다. 

### C++ 표준

http://eel.is/c++draft/basic.lval


            expression
    glvalue         rvalue

lvalue      xvalue      prvalue


i와 m으로 나누고, iI, mM의 조합으로 im, Im, iM, IM으로 구분해서 
생각할 수 있다. i가 glvalue, m이 rvalue, im이 xvalue, Im이 prvalue, 
iM이 lvaue이다. 

c++의 수식을 구성하는 요소 각각에서 분류가 결정된다. 

## decltype

```c++
template <typename T, typename U>
void add(T t, U u, decltype(t + u)* result) {
  *result = t + u;
}
```

## declval

생성자 호출 없이 타잎의 값을 얻어 오는 기능. 

```c++
std::declval<T>()
```

## 다시 돌아와서

```c++
template <typename... Args, typename S, typename Char = char_t<S>>
inline format_arg_store<buffer_context<Char>, remove_reference_t<Args>...>
make_args_checked(const S& format_str,
                  const remove_reference_t<Args>&... args) {
  static_assert(all_true<(!std::is_base_of<view, remove_reference_t<Args>>() ||
                          !std::is_reference<Args>())...>::value,
                "passing views as lvalues is disallowed");
  check_format_string<remove_const_t<remove_reference_t<Args>>...>(format_str);
  return {args...};
}

```

## variadic template 

```c++
template <typename T, typename... Types>
```
Types가 템플릿 파라미터 팩이고 typename...로 표시한다.

```c++
void print(T arg, Types... args) {
```
Types...도 파라미터 팩으로 불린다.

```c++
template <typename T>
void print(T arg) {
	std::cout << arg << std::endl;
}

template <typename T, typename... Types>
void print(T arg, Types... args) {
	std::cout << arg << ", ";
	print(args...);
}
```

args... 은 파라미터 팩 확장이다.


sizeof...

## fold 형식 

하스켈의 폴드 함수와 같은 기능이다.

```c++
#include <iostream>

template <typename... Ints>
int sum_all(Ints... nums) {
  return (... + nums);
}

int main() {
  // 1 + 4 + 2 + 3 + 10
  std::cout << sum_all(1, 4, 2, 3, 10) << std::endl;
}
```

이항 폴드 연산자도 있다. 
( E OP ... OP I)
( I OP ... OP E)

이건 매우 흥미롭다. 

```c++
class A {
 public:
  void do_something(int x) const {
    std::cout << "Do something with " << x << std::endl;
  }
};

template <typename T, typename... Ints>
void do_many_things(const T& t, Ints... nums) {
  // 각각의 인자들에 대해 do_something 함수들을 호출한다.
  (t.do_something(nums), ...);
}

int main() {
  A a;
  do_many_things(a, 1, 3, 2, 4);
}
```

fmt의 아이디어는 단순하지만 매우 빠르고 편리하다. 


