//-------------------------------------------------------------------------
// Name: Gizmo Lit Mesh
// Desc: This shader is used to draw gizmo lit meshes (e.g. arrow cones, 
//		 multi-axis translation squares, rotation sphere etc.).
//-------------------------------------------------------------------------
Shader "Gizmo Lit Mesh"
{
	Properties 
	{
		_Color ("Mesh Color", Color) = (1,1,1,1)			// The color of the mesh
		_StencilRefValue ("Stencil Ref Value", int) = 1		// We will use the stencil buffer to mark areas of the screen where lines
															// can no be drawn. This is necessary in order to avoid certain problems
															// which can occur when gizmo lines overlap with gizmo geometry.
	}

	SubShader 
	{
		// We want to support transparent geometry
		Tags { "Queue" = "Transparent" }

		Pass
		{
			// Setup the stencil operations
			Stencil
			{
				Ref [_StencilRefValue]		// Specify the reference value
				Comp Always					// The stencil test will always pass for meshes. The only purpose for which we are using a stencil
											// buffer is to mark areas of the screen where gizmo lines can not be drawn. 
				Pass Replace				// When the stencil test passes, we will replace the current value in the stencil buffer with the
											// specified reference value. When a gizmo line will be rendered, its shader will compare the stencil
											// buffer entry of each pixel in the line with the value written here. The pixel will only be accepted
											// if its reference value doesn't match the one written here.
			}

			// Each mesh vertex will be colored using the value of the color property
			Material
			{
				Diffuse[_Color]
			}

			ZTest Off							// We want gizmo meshes to render on top of everyting else
			ZWrite Off							// Stop writing to the ZBuffer. This is necessary in some specialized scenarios such as
												// allowing the object selection boxes to be seen when they are rendered behind the rotation 
												// gizmo sphere.
			Lighting On							// We want to have lighting applied to vertices
			Blend SrcAlpha OneMinusSrcAlpha		// Enable alpha blending for meshes with transparent vertices
		}
	} 
}
