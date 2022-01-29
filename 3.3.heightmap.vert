#version 330 core
layout (location = 0) in vec3 aPos;

uniform mat4 global_mat4 = mat4(1);
uniform mat4 view_mat4 = mat4(1);
uniform mat4 projection_mat4 = mat4(1);

out vec2 texCoords;
out vec4 color;

uniform sampler2D texture0;


void main()
{
    texCoords = vec2((aPos.z+1)/2, (aPos.x+1)/2);
    color = texture(texture0, texCoords);
    vec4 vertPos = vec4(aPos.x, aPos.y + (color.x + color.y + color.z) / 3 , aPos.z, 1.0);

    gl_Position = projection_mat4 * view_mat4 * global_mat4 * vertPos;
} 