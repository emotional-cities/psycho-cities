#version 400
uniform float color;
uniform vec4 color1 = vec4(1,0,0,1);
uniform vec4 color2 = vec4(0,1,0,1);
vec4 Result;
in vec2 texCoord;
out vec4 fragColor;




void main()

{
  Result = (1 - texCoord.x) * color1 + (texCoord.x) * color2;
  fragColor = vec4(Result);
}
