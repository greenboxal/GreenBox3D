﻿// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
//
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
//

in vec3 iPosition;
in vec3 iNormal;
in vec2 iTexCoord;

out vec2 gTexCoord;

void main()
{
	gTexCoord = iTexCoord;
	gl_Position = iPosition * pWorldViewProjection;
}
