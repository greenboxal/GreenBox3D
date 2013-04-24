﻿// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
//
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
//

in vec2 gTexCoord;

uniform sampler2D pTexture;

void main()
{
	gl_Color = tex(pTexture, gTexCoord);
}
