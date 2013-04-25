// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
//
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
//
//
// Basic shader test file
//
// TODO:
// * Implement Fallback
// * Handle HLSL
//

Shader "Simple"
{ 
	Version 330

	Input
	{
		iPosition : POSITION[0];
		iNormal : NORMAL[0];
		iColor : COLOR[0];
	}

	// GLSL shaders
	VertexGlsl "Simple/Simple.vert"
	PixelGlsl "Simple/Simple.frag"
			
	// HLSL shaders
	VertexHlsl "Simple/Simple.hlv"
	PixelHlsl "Simple/Simple.hlp"

	Fallback "None"
}
