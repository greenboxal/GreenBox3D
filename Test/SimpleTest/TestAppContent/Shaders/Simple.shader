// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
//
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
//
//
// Basic shader test file
//
// TODO:
// * Implement state changing capabilities
// * HLSL
//

Shader "Simple/Simple"
{ 
	Version 400
	Input
	{
		vec3 iPosition : POSITION[0];
		vec2 iNormal : NORMAL[0];
		vec2 iTexCoord : TEXCOORD[0];
	}
	Parameters
	{
		mat4 pWorldViewProjection;
		sampler2d pTexture;
	}
	Globals
	{
		vec2 gTexCoord;
	}
	Passes
	{
		Pass
		{
			// GLSL shaders
			VertexGlsl "Simple/Simple.vert"
			PixelGlsl "Simple/Simple.frag"
			
			// HLSL shaders
			VertexHlsl "Simple/Simple.hlv"
			PixelHlsl "Simple/Simple.hlp"
		}
	}
	Fallback "Simple/None"
}
