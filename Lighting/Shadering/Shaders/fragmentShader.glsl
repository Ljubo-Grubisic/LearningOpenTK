#version 330 core
struct Material{
	sampler2D diffuse;
	sampler2D specular;

	float shininess;
};

struct PointLight{
	vec3 position;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

	float constant;
	float linear;
	float quadratic;
};

struct DirLight{
	vec3 direction;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;
};

struct SpotLight{
	vec3 position;
	vec3 direction;

	float cutOff;
	float outerCutOff;

	vec3 ambient;
	vec3 diffuse;
	vec3 specular;

	float constant;
	float linear;
	float quadratic;
};

in vec3 Normal;
in vec3 FragPos;
in vec2 TexCoords;

uniform vec3 viewPos; 
uniform Material material;

#define numPointLights 4

uniform PointLight pointLights[numPointLights];
uniform DirLight dirLight;
uniform SpotLight spotLight;

out vec4 outputColor;

vec3 CalculatePointLight(PointLight light, Material material, vec3 normal, vec3 viewDir){
	// Ambient
	vec3 ambient = texture(material.diffuse, TexCoords).rgb * light.ambient;

	// Diffuse
	vec3 lightDir = normalize(light.position - FragPos);

	float diff = max(dot(normal, lightDir), 0.0);
	vec3 diffuse = (diff * texture(material.diffuse, TexCoords).rgb) * light.diffuse;
	
	// Specular
	vec3 reflectDir = normalize(reflect(-lightDir, normal));
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	vec3 specular = (spec * texture(material.specular, TexCoords).rgb) * light.specular;

	// Attenuation
	float distance = length(FragPos - light.position);
	float attenuation = 1 / (light.constant + light.linear * distance + pow(light.quadratic, 2) * distance);

	ambient *= attenuation;
	diffuse *= attenuation;
	specular *= attenuation;

	return (ambient + diffuse + specular);
}

vec3 CalculateDirLight(DirLight light, Material material, vec3 normal, vec3 viewDir){
	// Ambient
	vec3 ambient = texture(material.diffuse, TexCoords).rgb * light.ambient;

	// Diffuse
	vec3 lightDir = normalize(-light.direction);

	float diff = max(dot(normal, lightDir), 0.0);
	vec3 diffuse = (diff * texture(material.diffuse, TexCoords).rgb) * light.diffuse;
	
	// Specular
	vec3 reflectDir = normalize(reflect(-lightDir, normal));
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
	vec3 specular = (spec * texture(material.specular, TexCoords).rgb) * light.specular;

	return (ambient + diffuse + specular);
};

vec3 CalculateSpotLight(SpotLight light, Material material, vec3 normal, vec3 viewDir){
	 vec3 lightDir = normalize(light.position - FragPos);
    // diffuse shading
    float diff = max(dot(normal, lightDir), 0.0);
    // specular shading
    vec3 reflectDir = reflect(lightDir, normal);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), material.shininess);
    // attenuation
    float distance = length(light.position - FragPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance + light.quadratic * (distance * distance));
    // intensity
    float theta = dot(lightDir, normalize(-light.direction)); 
    float epsilon = light.cutOff - light.outerCutOff;
    float intensity = clamp((theta - light.outerCutOff) / epsilon, 0.0, 1.0);

    vec3 ambient = light.ambient * texture(material.diffuse, TexCoords).rgb;
    vec3 diffuse = light.diffuse * diff * texture(material.diffuse, TexCoords).rgb;
    vec3 specular = light.specular * spec * texture(material.specular, TexCoords).rgb;

    ambient *= attenuation * intensity;
    diffuse *= attenuation * intensity;
    specular *= attenuation * intensity;

    return (ambient + diffuse + specular);
};

void main()
{
	vec3 normal = normalize(Normal);
	vec3 viewDir = normalize(viewPos - FragPos);

	vec3 result = CalculateSpotLight(spotLight, material, normal, viewDir);
	
	result += CalculateDirLight(dirLight, material, normal, viewDir);

	for(int i = 0; i < numPointLights; i++){
		result += CalculatePointLight(pointLights[i], material, normal, viewDir);
	}

	outputColor = vec4(result, 1.0);
}