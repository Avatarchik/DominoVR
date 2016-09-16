//-------------------------------------------------------------------------
// Name: Gizmo Line Rendering
// Desc: This shader is used to draw gimzo lines (e.g. translation gizmo axes,
//		 rotation gizmo circle lines, scale gizmo axes etc).
//-------------------------------------------------------------------------
Shader "Gizmo Line Rendering"
{
	Properties
	{
		_Color ("Line Color", Color) = (1.0, 1.0, 1.0, 1.0)		// The color of a single line vertex which is processed by the shader
		_StencilRefValue ("Stencil Ref Value", int) = 1			// We will use the stencil buffer to mark areas on the screen where the line
																// can not be drawn. This value can be set from the client code to allow for
																// more flexibility.
	}

	SubShader
	{
		// We want to allow the usage of transparent line vertices
		Tags { "Queue" = "Transparent" }

		Pass
		{
			// Setup the stencil operations accordingly
			Stencil
			{
				Ref [_StencilRefValue]							// Specify the reference value
				Comp NotEqual									// The line pixel is only accepted if the reference value is not equal the one which
			}

			// Each line vertex will be colored using the value of the color property
			Material
			{
				Diffuse[_Color]
			}

			Lighting Off						// No light is needed when rendering lines
			ZTest Off							// We want to render the gizmos on top of everything else, so the depth test must be disabled
			Blend SrcAlpha OneMinusSrcAlpha		// Enable alpha blending for lines with transparent vertices
		}
	}
}
