#version 430 core

layout (location = 0) in vec3 position;
layout (location = 1) in vec3 normal; 
layout (location = 2) in vec4 color; 
layout (location = 3) in vec2 tex1;

layout (location =12) uniform mat4 mvpMatrix;

out Vertex {
    vec2 vTex1;
    vec4 vColor;
} vert;

void main()
{
    vert.vTex1 = tex1;
    vert.vColor = color;

    gl_Position = mvpMatrix * vec4(position, 1);
}