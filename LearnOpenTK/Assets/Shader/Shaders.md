# Shaders 

## diffuse

- 매우 이상한 현상이다.
  - 문법이 정확해 보이는데 컴파일 에러가 난다.
  - 그것도 어디선가 잘린 듯한 현상이다.

```c#
// http://shader-playground.timjones.io/. 쉐이더 오류 체크

#version 430 core
 
// 버텍스당 입력
layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal;
layout (location = 2) in vec4 color;
layout (location = 3) in vec2 texcoord;

layout (location = 12) uniform mat4 mvp_matrix;
 
// 프래그먼트 쉐이더로의 출력
out VS_OUT
{
  vec2 uv;
  vec4 col;
} vs_out;
 
// 버텍스 쉐이더의 시작점
void main(void)
{
  vs_out.col = color;


  vs_out.uv = texcoord;

  gl_Position = mvp_matrix * vec4(position, 1);
}
```

다시 타이핑을 해서 입력하고 VS_OUT 대신에 varying을 사용했다. 

## 교재 

https://thebookofshaders.com/
기본 내용에 충실하면서 glsl로 진행한다. 
glsl은 vulkan에서 계속 지원하므로 투자가 유효하다.
무엇보다 개념은 계속 지원될 것이다. 

교재 내용을 짬짬이 공부해 나간다. 

