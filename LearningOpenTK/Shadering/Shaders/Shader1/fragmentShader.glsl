#version 330 core
out vec4 outputColor;

//in vec4 Color;
in vec2 TexCoords;

uniform sampler2D texture1;
uniform sampler2D texture2;

void main()
{
	outputColor = mix(texture(texture1, TexCoords), texture(texture2, TexCoords), 0.2);
}
//outputColor = mix(texture(texture1, TexCoords), texture(texture2, TexCoords), 0.2);