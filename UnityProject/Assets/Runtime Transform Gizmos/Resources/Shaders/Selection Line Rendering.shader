//-------------------------------------------------------------------------
// Name: Selection Line Rendering
// Desc: This shader is used to draw lines in the context of object selection.
//-------------------------------------------------------------------------
Shader "Selection Line Rendering"
{
	Properties
	{
		_Color ("Line Color", Color) = (1.0, 1.0, 1.0, 1.0)		// The color of a single line vertex which is processed by the shader
	}

	SubShader
	{
		// We want to allow the usage of transparent line vertices
		Tags { "Queue" = "Transparent" }

		Pass
		{
			// Each line vertex will be colored using the value of the color property
			Material
			{
				Diffuse[_Color]
			}

			ZTest On							// We want the lines to be obscured by objects in the scene
			Lighting Off						// No light is needed when rendering lines
			Blend SrcAlpha OneMinusSrcAlpha		// Enable alpha blending for lines with transparent vertices
		}
	}
}
