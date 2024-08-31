#version 330 core


in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

uniform vec3 viewPos; 
uniform sampler2D texture1;

out vec4 outputColor;

float near = 0.1;
float far = 100.0;

float LinearizeDepth(float depth)
{
	float z = depth * 2.0 - 1.0;
	return (2.0 * near * far) / (far + near - z * (far - near));
}


void main()
{
	outputColor = texture(texture1, TexCoords);
}