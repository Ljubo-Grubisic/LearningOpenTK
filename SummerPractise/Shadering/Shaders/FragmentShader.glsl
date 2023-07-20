#version 330 core
in vec2 Tex;                        

out vec4 FragColor;

uniform sampler2D texture1;
uniform sampler2D texture2;

void main()
{
    FragColor = mix(texture(texture1, Tex), texture(texture2, Tex), 0.2);
}
                                        
