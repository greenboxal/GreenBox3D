// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
//
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
//

in vec3 VertexPosition;
in vec2 VertexTextureCoord;

out vec2 TexCoord0;

uniform mat4 WVP;

void main()
{
	TexCoord0 = VertexTextureCoord;
	gl_Position = WVP * vec4(VertexPosition, 1);
}
