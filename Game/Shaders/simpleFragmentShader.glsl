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
	//float depth = LinearizeDepth(gl_FragCoord.z) / far;
	//outputColor = vec4(vec3(depth), 1.0);
	outputColor = vec4(texture(texture1, TexCoords).rgb, 1.0);
}