// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
//
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
//

in vec3 iPosition;
in vec3 iNormal;
in vec4 iColor;

out vec4 gColor;

uniform mat4 WorldViewProjection;

void main()
{
	gColor = iColor;
	gl_Position = WorldViewProjection * vec4(iPosition, 1);
}
