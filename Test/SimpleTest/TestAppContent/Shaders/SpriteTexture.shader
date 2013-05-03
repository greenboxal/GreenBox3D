// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
//
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
//

Shader "SpriteTexture"
{ 
	Version 130

	Input
	{
		VertexPosition : POSITION[0];
		VertexTextureCoord : TEXCOORD[0];
	}

	// GLSL shaders
	VertexGlsl "SpriteTexture/SpriteTexture.vert"
	PixelGlsl "SpriteTexture/SpriteTexture.frag"
			
	// HLSL shaders
	VertexHlsl "SpriteTexture/SpriteTexture.hlv"
	PixelHlsl "SpriteTexture/SpriteTexture.hlp"

	Fallback "None"
}
