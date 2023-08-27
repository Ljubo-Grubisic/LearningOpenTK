#version 330 core
struct Material
{
	sampler2D texture_diffuse1;
	sampler2D texture_specular1;
};
in vec2 TexCoords;

uniform Material material;

out vec4 outputColor;

void main()
{
	outputColor = texture(material.texture_diffuse1, TexCoords);
}