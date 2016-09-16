//-------------------------------------------------------------------------
// Name: Gizmo Unlit Mesh
// Desc: This shader is used to draw gizmo unlit meshes (e.g. arrow cones, 
//		 rotation sphere etc.).
//-------------------------------------------------------------------------
Shader "Gizmo Unlit Mesh"
{
	Properties 
	{
		_Color ("Mesh Color", Color) = (1,1,1,1)		// The color of the mesh
	}

	SubShader 
	{
		// We want to support transparent geometry
		Tags { "Queue" = "Transparent" }

		// Use the specified color property to color the vertices
		Color[_Color]

		Pass
		{
			ZTest Off							// We want gizmo meshes to render on top of everyting else
			ZWrite Off							// Stop writing to the ZBuffer. This is necessary in some specialized scenarios such as
												// allowing the object selection boxes to be seen when they are rendered behind the rotation 
												// gizmo sphere.						
			Cull Off							// Do not use culling for unlit meshes. This is useful for certain types if geometry like 
												// planes (e.g. multi-axis translation squares for the translation gizmo).
			Blend SrcAlpha OneMinusSrcAlpha		// Enable alpha blending for meshes with transparent vertices
		}
	} 
}
