using UnityEngine;
using System.Collections.Generic;
using RTEditor;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// This is a static class that implements some useful 'GameObject' extension methods.
/// </summary>
public static class GameObjectExtensions
{
    #region Public Static Functions
    #if UNITY_EDITOR
    /// <summary>
    /// Can be used to check if the specified game object is a scene object. This is useful
    /// when wanting to avoid assigning prefabs to 'GameObject' references.
    /// </summary>
    public static bool IsSceneObject(this GameObject gameObject)
    {
        PrefabType prefabType = PrefabUtility.GetPrefabType(gameObject);

        // Make sure the specified game object is not a prefab. A game object is not a prefab if
        // 'prefabType' is either 'None' or if the value specifies that object is some kind of
        // a prefab instance. If it is an instance it means it exists inside the scene.
        return prefabType == PrefabType.None || prefabType == PrefabType.PrefabInstance ||
               prefabType == PrefabType.DisconnectedPrefabInstance || prefabType == PrefabType.MissingPrefabInstance;
    }
    #endif

    /// <summary>
    /// Static function which can be used to retrieve a hash set of all root/top parent
    /// objects from the specified object collection.
    /// </summary>
    public static HashSet<GameObject> GetRootObjectsFromObjectCollection(List<GameObject> gameObjects)
    {
        // Loop through all game objects and store their roots inside the set
        var objectRoots = new HashSet<GameObject>();
        foreach (GameObject gameObject in gameObjects)
        {
            objectRoots.Add(gameObject.transform.root.gameObject);
        }

        return objectRoots;
    }

    /// <summary>
    /// Given a collection of game objects, the function returns a list that contains the
    /// objects in the list that don't have any parents inside 'gameObjects'. For example,
    /// if the specified object collection contains the objects A, B, C, D, E and F, where
    /// A is the parent of B and C and D is the parent of E and F, the function will return
    /// the objects A and D. The parents of A and D (if any) are not taken into account.
    /// </summary>
    public static List<GameObject> GetParentsFromObjectCollection(IEnumerable<GameObject> gameObjects)
    {
        // Loop through all game objects
        var parents = new List<GameObject>();
        foreach (GameObject currentObject in gameObjects)
        {
            // Cache the object's transform for easy and fast access
            Transform currentObjectTransform = currentObject.transform;

            // Assume the object doesn't have a parent
            bool foundParent = false;

            // Now loop through the game object collection again and check if the object has a parent
            foreach (GameObject gameObject in gameObjects)
            {
                // Same object?
                if (gameObject != currentObject)
                {
                    // If the current game object has a parent, we set the 'foundParent' variable to
                    // true and exit the loop because we found the info that we were interested in.
                    if (currentObjectTransform.IsChildOf(gameObject.transform))
                    {
                        foundParent = true;
                        break;
                    }
                }
            }

            // If no parent was found, add the game object to the top parents list
            if (!foundParent) parents.Add(currentObject);
        }

        return parents;
    }

    /// <summary>
    /// Static function which can be used to retrieve a hash set of all root/top parent
    /// objects from the specified object collection.
    /// </summary>
    public static HashSet<GameObject> GetRootObjectsFromObjectCollection(HashSet<GameObject> gameObjects)
    {
        // Loop through all game objects and store their roots inside the set
        var objectRoots = new HashSet<GameObject>();
        foreach (GameObject gameObject in gameObjects)
        {
            objectRoots.Add(gameObject.transform.root.gameObject);
        }

        return objectRoots;
    }

    /// <summary>
    /// Returns a list of of all children of 'gameObject' including 'gameObject' itself.
    /// </summary>
    public static List<GameObject> GetAllChildrenIncludingSelf(this GameObject gameObject)
    {
        // Retrieve all child transforms
        Transform[] allChildTransforms = gameObject.GetComponentsInChildren<Transform>();
        var gameObjects = new List<GameObject>(allChildTransforms.Length);

        // Loop through all child transforms and add them to the output list
        foreach(Transform childTransform in allChildTransforms)
        {
            gameObjects.Add(childTransform.gameObject);
        }

        return gameObjects;
    }

    /// <summary>
    /// Sets the absolute scale of the specified game object.
    /// </summary>
    public static void SetAbsoluteScale(this GameObject gameObject, Vector3 absoluteScale)
    {
        // Cache needed data
        Transform objectTransform = gameObject.transform;
        Transform oldParent = objectTransform.parent;

        // In order to set the absolute scale, we will first detach the object from its
        // parent and then use the 'localScale' property to set the scale. We then attach
        // the game object to its parent again.
        objectTransform.parent = null;
        objectTransform.localScale = absoluteScale;
        objectTransform.parent = oldParent;
    }

    /// <summary>
    /// Rotates 'gameObject' around 'rotationAxis' by 'angleInDegrees'. 
    /// </summary>
    /// <remarks>
    /// The function rotates both the object's orientation and its position.
    /// </remarks>
    /// <param name="gameObject">
    /// The game object which must be rotated.
    /// </param>
    /// <param name="rotationAxis">
    /// The rotation axis. The function assumes the rotation axis is normalized.
    /// </param>
    /// <param name="angleInDegrees">
    /// The angle of rotation in degrees.
    /// </param>
    /// <param name="pivotPoint">
    /// This is point around which the rotation is performed.
    /// </param>
    public static void Rotate(this GameObject gameObject, Vector3 rotationAxis, float angleInDegrees, Vector3 pivotPoint)
    {
        // Cache needed data
        Transform objectTransform = gameObject.transform;

        // Calculate the vector which holds the relationship between the pivot point and the object's position
        Vector3 fromPivotToPosition = objectTransform.position - pivotPoint;

        // Rotate the relationship vector. We need to do this because after the rotation is applied to the
        // game object, we need to also adjust its position in such a way that the rotation happens around
        // the pivot point.
        Quaternion rotationQuaternion = Quaternion.AngleAxis(angleInDegrees, rotationAxis);
        fromPivotToPosition = rotationQuaternion * fromPivotToPosition;

        // Rotate the object's local coordinate system
        objectTransform.Rotate(rotationAxis, angleInDegrees, Space.World);

        // Now adjust the position. The new position is the pivot point + the transformed relationshop vector. This
        // has the effect of locking the object's position to the tip of the vector which unites the pivot and the
        // position. Regardless of how the relationshop vector is rotated, the position vector will rotate with it 
        // around the pivot point.
        objectTransform.position = pivotPoint + fromPivotToPosition;
    }

    /// <summary>
    /// Returns the specified game object's screen rectangle.
    /// </summary>
    /// <param name="camera">
    /// The camera which renders the game object.
    /// </param>
    /// <remarks>
    /// If the specified game object doesn't have a mesh attached to it or a box,
    /// sphere or capsule collider, the function will return an empty rectangle.
    /// </remarks>
    public static Rect GetScreenRectangle(this GameObject gameObject, Camera camera)
    {
        // Retrieve the game object's world space AABB
        Bounds worldSpaceAABB = gameObject.GetWorldSpaceAABB();
        if (!worldSpaceAABB.IsValid()) return new Rect(0.0f, 0.0f, 0.0f, 0.0f);

        // Return the rectangle which encloses the world space AABB in screen space
        return worldSpaceAABB.GetScreenRectangle(camera);
    }

    /// <summary>
    /// Returns the world space AABB of the specified game object.
    /// </summary>
    public static Bounds GetWorldSpaceAABB(this GameObject gameObject)
    {
        // First, retrieve the game object's model space AABB
        Bounds modelSpaceAABB = gameObject.GetModelSpaceAABB();
        if (!modelSpaceAABB.IsValid()) return BoundsExtensions.GetInvalidBoundsInstance();

        // Transform the AABB in world space and return it to the caller
        return modelSpaceAABB.Transform(gameObject.transform.localToWorldMatrix);
    }

    /// <summary>
    /// Given a game object, the function will assign it and its children to the specified layer.
    /// </summary>
    public static void SetLayerForEntireHierarchy(this GameObject gameObject, int layer)
    {
        // Assign the specified game object to the specified layer
        gameObject.layer = layer;

        // Assign the objects children to the specified layer
        Transform[] transforms = gameObject.GetComponentsInChildren<Transform>();
        foreach(Transform objectTransform in transforms)
        {
            objectTransform.gameObject.layer = layer;
        }
    }

    /// <summary>
    /// Returns the model space AABB of the specified game object. If the game object
    /// has a mesh, the function will return the mesh's local AABB. Otherwise, it will
    /// return the local space AABB of the first available collider using the following
    /// priority for collider types: box, sphere, capsule. If the game object doesn't
    /// have a collider either, the function will return an invalid AABB.
    /// </summary>
    /// <remarks>
    /// The method will only take into consideration the following collider types: box,
    /// sphere and capsule.
    /// </remarks>
    public static Bounds GetModelSpaceAABB(this GameObject gameObject)
    {
        // First, attempt to retrieve the object's model space mesh AABB
        Bounds modelSpaceAABB = gameObject.GetModelSpaceMeshAABB();
        if (modelSpaceAABB.IsValid()) return modelSpaceAABB;

        // If no mesh is present, then try to retrieve the model space collider AABB
        return gameObject.GetModelSpaceColliderAABB();
    }

    /// <summary>
    /// Returns the model space AABB of the hierarchy whose root is 'gameObject'.
    /// </summary>
    public static Bounds GetHierarchyModelSpaceAABB(this GameObject gameObject)
    {
        // Retrieve all child transforms
        Transform rootTransform = gameObject.transform;
        Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();

        // Initialize the hierarchy model space AAB with the model space AABB of the root.
        // This AABB will be adjusted to encapsulate the AABB of the entire object hierarchy.
        Bounds hierarchyModelSpaceAABB = gameObject.GetModelSpaceAABB();

        // Loop through all child transforms
        foreach(Transform childTransform in childTransforms)
        {
            // Skip the root transform because we already handled this outside the 'foreach' loop
            if(rootTransform != childTransform)
            {
                // Store child object for easy access
                GameObject childObject = childTransform.gameObject;

                // Retrieve the child's model space AABB. If it is valid, we can continue. Otherwise, we ignore it and move on.
                Bounds childModelSpaceAABB = childObject.GetModelSpaceAABB();
                if(childModelSpaceAABB.IsValid())
                {
                    // The AABB is valud. The next step is to calculate a transform matrix which places the child's
                    // AABB in hierarchy model space. This is the matrix which contains the transform relative to the
                    // root of the hierarchy.
                    Matrix4x4 rootRelativeTransformMatrix = childTransform.GetRelativeTransform(rootTransform);
                    childModelSpaceAABB = childModelSpaceAABB.Transform(rootRelativeTransformMatrix);

                    // If the hierarchy AABB is valid, we can perform a merge. Otherwise, we must initialize it.
                    if (hierarchyModelSpaceAABB.IsValid()) hierarchyModelSpaceAABB.Encapsulate(childModelSpaceAABB);
                    else hierarchyModelSpaceAABB = childModelSpaceAABB;
                }
            }
        }

        return hierarchyModelSpaceAABB;
    }

    /// <summary>
    /// Returns the model space collider AABB of the specified game object. The method
    /// only works with box, sphere and capsule colliders. If more than one collider type
    /// is attached to the game object, the method will return the first available collider
    /// using the following priority: box, sphere, capsule.
    /// </summary>
    public static Bounds GetModelSpaceColliderAABB(this GameObject gameObject)
    {
        // Find the first available collider based on the following priority: box, sphere, capsule
        Collider collider = gameObject.GetComponent<BoxCollider>();
        if (collider == null) collider = gameObject.GetComponent<SphereCollider>();
        if (collider == null) collider = gameObject.GetComponent<CapsuleCollider>();

        // If a collider was found, use its bounds to return the model space AABB. If no
        // collider was found, we will return an invalid bounds instance.
        if (collider != null) return new Bounds(Vector3.zero, collider.bounds.size);
        else return BoundsExtensions.GetInvalidBoundsInstance();
    }

    /// <summary>
    /// Returns the model space mesh AABB of the specified game object. If the object
    /// has both a mesh filter and a skinned mesh renderer component attached to it, 
    /// both with valid meshes, the AABB of the mesh which is assigned to the mesh
    /// filter will be returned.
    /// </summary>
    public static Bounds GetModelSpaceMeshAABB(this GameObject gameObject)
    {
        // Check if the object has a mesh filter component with a valid mesh attached to it
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null) return meshFilter.sharedMesh.bounds;

        // The game object may have a skinned mesh renderer with a valid mesh
        SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null) return skinnedMeshRenderer.localBounds;

        // No mesh was found; return an invalid AABB
        return BoundsExtensions.GetInvalidBoundsInstance();
    }

    /// <summary>
    /// Returns the mesh attached to the specified game object. If the game object has both
    /// a mesh filter and a skinned mesh renderer attached to it, the mesh associated with the
    /// mesh filter will be returned if one is present. Otherwise, the skinned mesh renderer's
    /// mesh will be returned. If none of them have a valid mesh, or if no mesh filter or
    /// skinned mesh renderer are present, the function will return null.
    /// </summary>
    public static Mesh GetMesh(this GameObject gameObject)
    {
        // Check if the object has a mesh filter component with a valid mesh attached to it
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null) return meshFilter.sharedMesh;

        // The game object may have a skinned mesh renderer with a valid mesh
        SkinnedMeshRenderer skinnedMeshRenderer = gameObject.GetComponent<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null) return skinnedMeshRenderer.sharedMesh;

        return null;
    }

    /// <summary>
    /// Removes all attached colliders from the specified game object.
    /// </summary>
    public static void RemoveAllColliders(this GameObject gameObject)
    {
        // Loop through all colliders and destroy them
        Collider[] allColliders = gameObject.GetComponents<Collider>();
        foreach(Collider collider in allColliders)
        {
            // Destory collider
            #if UNITY_EDITOR
            RuntimeEditorApplication.DestroyImmediate(collider);
            #else
            RuntimeEditorApplication.Destroy(collider);
            #endif
        }
    }

    /// <summary>
    /// Destroys all children of the specified game object.
    /// </summary>
    public static void DestroyAllChildren(this GameObject gameObject)
    {
        // Loop through all child transforms
        Transform objectTransform = gameObject.transform;
        Transform[] allChildTransforms = gameObject.GetComponentsInChildren<Transform>();
        foreach(Transform childTransform in allChildTransforms)
        {
            // Same as parent object?
            if (objectTransform == childTransform) continue;

            // Destroy object
            #if UNITY_EDITOR
            RuntimeEditorApplication.DestroyImmediate(childTransform.gameObject);
            #else
            RuntimeEditorApplication.Destroy(childTransform.gameObject);
            #endif
        }
    }

    /// <summary>
    /// Returns all the light components which are attached to the specified game object. If
    /// no light component is attached to the game object, the returned array will be empty.
    /// </summary>
    public static Light[] GetAllLightComponents(this GameObject gameObject)
    {
        return gameObject.GetComponents<Light>();
    }

    /// <summary>
    /// Returns the first encountered light component which is attached to the specified game
    /// object. If no light component is attached to the game object, the function will return
    /// null.
    /// </summary>
    public static Light GetFirstLightComponent(this GameObject gameObject)
    {
        Light[] allLightComponents = gameObject.GetAllLightComponents();
        if (allLightComponents.Length != 0) return allLightComponents[0];

        return null;
    }

    /// <summary>
    /// Returns all the particle system components which are attached to the specified game object. If
    /// no particle system component is attached to the game object, the returned array will be empty.
    /// </summary>
    public static ParticleSystem[] GetAllParticleSystemComponents(this GameObject gameObject)
    {
        return gameObject.GetComponents<ParticleSystem>();
    }

    /// <summary>
    /// Returns the first encountered particle system component which is attached to the specified game
    /// object. If no particle system component is attached to the game object, the function will return
    /// null.
    /// </summary>
    public static ParticleSystem GetFirstParticleSystemComponent(this GameObject gameObject)
    {
        ParticleSystem[] allParticleSystemComponents = gameObject.GetAllParticleSystemComponents();
        if (allParticleSystemComponents.Length != 0) return allParticleSystemComponents[0];

        return null;
    }
    #endregion
}
