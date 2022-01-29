#version 330 core
out vec4 FragColor;

in vec2 texCoords;
in vec4 color;
uniform sampler2D texture0;

void main()
{
	FragColor =  color;
	//FragColor =  vec4(texCoords.x*20, 0, texCoords.y*20, 255);
	//FragColor =  vec4(texCoords.x, 0, 0, 255);
}