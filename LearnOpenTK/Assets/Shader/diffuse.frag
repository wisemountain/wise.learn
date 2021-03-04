#version 430 core

uniform sampler2D tex1;

in Vertex {
    vec2 vTex1;
    vec4 vColor;
} vert;

void main()
{
    // gl_FragColor = texture(tex1, vert.vTex1);
    gl_FragColor = vert.vColor;
}