//-------------------------------------------------------------------------
// Name: 2D Geometry Rendering
// Desc: This shader will be used to draw any necessary 2D geometry. For example,
//		 we will use this to draw 2D screen space qauds and the like.
//-------------------------------------------------------------------------
Shader "2D Geometry Rendering"
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
			ZTest Off							// We will need our 2D geometry to render on top of everything else, so we will disable Z tests
			Cull Off							// We will not concern ourselves with culling when rendering 2D geometry, so we will turn it off
			Blend SrcAlpha OneMinusSrcAlpha		// Enable alpha blending for meshes with transparent vertices
		}
	} 
}
