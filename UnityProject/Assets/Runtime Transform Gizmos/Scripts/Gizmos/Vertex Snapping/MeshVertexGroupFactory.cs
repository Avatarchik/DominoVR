using UnityEngine;
using System.Collections.Generic;

namespace RTEditor
{
    /// <summary>
    /// This class can be used to build a list of mesh vertex groups for a specified input mesh.
    /// </summary>
    public static class MeshVertexGroupFactory
    {
        #region Public Static Functions
        /// <summary>
        /// Creates and returns a list of vertex groups for the specified mesh.
        /// </summary>
        public static List<MeshVertexGroup> Create(Mesh mesh)
        {
            // These variables holds the number of groups per world unit. It's probably worth
            // experimenting with these values, but there is no correct value that you can set.
            // The bigger the values, the bigger the number of vertices which can exist in one
            // group. The smaller the value, the bigger the number of vertex groups. It all
            // depends on the kind of meshes you are dealing with. Setting this to 2, seems to
            // provide reasonably good results.
            const float numberOfGroupsPerWorldUnitX = 2.0f;
            const float numberOfGroupsPerWorldUnitY = 2.0f;
            const float numberOfGroupsPerWorldUnitZ = 2.0f;

            // Cache needed data
            Bounds meshBounds = mesh.bounds;
            Vector3 meshBoundsSize = meshBounds.size;
            Vector3[] meshVertices = mesh.vertices;

            // Calculate the vertec group size on all axes
            float vertexGroupSizeX = meshBoundsSize.x / numberOfGroupsPerWorldUnitX;
            float vertexGroupSizeY = meshBoundsSize.y / numberOfGroupsPerWorldUnitY;
            float vertexGroupSizeZ = meshBoundsSize.z / numberOfGroupsPerWorldUnitZ;

            // Store the size values inside a vector. This will be useful when building imaginary
            // AABBs for the vertex groups inside the nested 'for' loops defined below.
            Vector3 vertexGroupSize = new Vector3(vertexGroupSizeX, vertexGroupSizeY, vertexGroupSizeZ);
            Vector3 vertexGroupHalfSize = vertexGroupSize * 0.5f;
           
            // Calculate the number of groups on each axis.
            // Note: We add one because there may be cases in which the result of the inner multiplication operation
            //       might result in something like 1.34 for example. In that case it means we need an additional group
            //       to cover that additional 34% space. We also add 0.5 to the multiplication result in order to account
            //       for floating point rounding errors that could cause problems. For example, the multiplication may
            //       result in a value of 0.99999. In this case the final result would be 1, when in fact it was supposed
            //       to be 2.
            int numberOfGroupsOnX = (int)(numberOfGroupsPerWorldUnitX * meshBoundsSize.x + 0.5f) + 1;
            int numberOfGroupsOnY = (int)(numberOfGroupsPerWorldUnitY * meshBoundsSize.y + 0.5f) + 1;
            int numberOfGroupsOnZ = (int)(numberOfGroupsPerWorldUnitZ * meshBoundsSize.z + 0.5f) + 1;

            // Loop through each group (from bottom to top -> from back to front -> from left to right)
            var meshVertexGroups = new List<MeshVertexGroup>();
            for(int groupIndexY = 0; groupIndexY < numberOfGroupsOnY; ++groupIndexY)
            {
                // Cache needed data and loop from back to front
                float offsetAlongY = meshBounds.min.y + vertexGroupHalfSize.y + groupIndexY * vertexGroupSizeY;
                for(int groupIndexZ = 0; groupIndexZ < numberOfGroupsOnZ; ++groupIndexZ)
                {
                    // Cache needed data and loop from left to right
                    float offsetAlongZ = meshBounds.min.z + vertexGroupHalfSize.z + groupIndexZ * vertexGroupSizeZ;
                    for(int groupIndexX = 0; groupIndexX < numberOfGroupsOnX; ++groupIndexX)
                    {
                        // Calculate the center of the vertex group AABB using the current group indices
                        Vector3 vertexGroupAABBCenter = new Vector3(meshBounds.min.x + vertexGroupHalfSize.x + groupIndexX * vertexGroupSizeX,
                                                                    offsetAlongY, offsetAlongZ);
                        
                        // Calculate the group AABB
                        Bounds vertexGroupAABB = new Bounds(vertexGroupAABBCenter, vertexGroupSize);
                       
                        // Identify the vertices which reside inside the current group. These are the vertices which
                        // lie inside the calculated AABB.
                        List<Vector3> verticesInGroup = new List<Vector3>(meshVertices.Length / 2);
                        foreach(Vector3 vertex in meshVertices)
                        {
                            if (vertexGroupAABB.Contains(vertex)) verticesInGroup.Add(vertex);
                        }

                        // If there are any vertices which exist inside the group's AABB, we can create a new group
                        if (verticesInGroup.Count != 0)
                        {
                            var meshVertexGroup = new MeshVertexGroup(verticesInGroup);                      
                            meshVertexGroups.Add(meshVertexGroup);
                        }
                    }
                }
            }

            // Return the mesh vertex groups
            return meshVertexGroups;
        }
        #endregion
    }
}
