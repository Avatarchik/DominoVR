using UnityEngine;
using System.Collections.Generic;
using RTEditor;

/// <summary>
/// This is a static class that implements some useful 'Camera' extension methods.
/// </summary>
public static class CameraExtensions
{
    #region Public Static Functions
    /// <summary>
    /// Returns the view volume for the specified camera.
    /// </summary>
    public static CameraViewVolume GetViewVolume(this Camera camera)
    {
        // Create the view volume
        var viewVolume = new CameraViewVolume();
        viewVolume.BuildForCamera(camera);

        // Return the view volume to the caller
        return viewVolume;
    }

    /// <summary>
    /// Returns a list of game objects which are visible to the specified camera.
    /// </summary>
    /// <remarks>
    /// This function detects only objects which have a collider attached to them.
    /// </remarks>
    public static List<GameObject> GetVisibleGameObjects(this Camera camera)
    {
        // We need the camera view volume to detect the visible game objects
        var cameraViewVolume = new CameraViewVolume();
        cameraViewVolume.BuildForCamera(camera);

        // In order to detect the visible game objects, we will loop through all POTTENTIALLY visible
        // game objects and check if their AABB lies inside the camera frustum.
        List<GameObject> pottentiallyVisibleGameObjects = camera.GetPottentiallyVisibleGameObjects();
        var visibleGameObjects = new List<GameObject>(pottentiallyVisibleGameObjects.Count);        // Set initial capacity to avoid resize
        foreach(GameObject gameObject in pottentiallyVisibleGameObjects)
        {
            // If the game object's world space AABB intersects the camera frustum, it means it is visible
            Bounds worldSpaceAABB = gameObject.GetWorldSpaceAABB();
            if (cameraViewVolume.ContainsWorldSpaceAABB(worldSpaceAABB)) visibleGameObjects.Add(gameObject);
        }
       
        // Return the visible objects list to the caller
        return visibleGameObjects;
    }
    #endregion

    #region Private Static Functions
    /// <summary>
    /// Returns a list of game objects which COULD be visible to the specified camera. Essentially,
    /// the function gathers the game objects which sit around the camera and which might be visible
    /// when rendered.
    /// </summary>
    /// <remarks>
    /// This function detects only objects which have a collider attached to them.
    /// </remarks>
    private static List<GameObject> GetPottentiallyVisibleGameObjects(this Camera camera)
    {
        // Cache needed data
        Transform cameraTransform = camera.transform;
        CameraViewVolume cameraViewVolume = camera.GetViewVolume();

        // We will use 'Physics.OverlapSphere' to gather the list of pottentially visible game objects. So we will need to 
        // construct a sphere that surrounds the camera view volume. Some of the objects which reside inside the sphere may 
        // not be visible but we will have the guarantee that the objects which are visible (reside inside the camera view 
        // volume) will be overlapped by the sphere. We will construct the sphere such that its center sits at half way distance
        // between the camera position and its far clip plane and the sphere radius is set to the magnitude of the vector which 
        // unites the sphere center and the top left point on the camera far clip plane. Calculating the radius like this will 
        // ensure that the sphere will cover the entire view volume.
        Vector3 sphereCenter = cameraTransform.position + cameraTransform.forward * camera.farClipPlane * 0.5f;
        float sphereRadius = (cameraViewVolume.TopLeftPointOnFarPlane - sphereCenter).magnitude * 1.01f;        // Increase the radius a tad just to be sure :)

        // Use the 'Physics.OverlapSphere' function to gather a list of objects which MAY be visible
        Collider[] overlappedColliders = Physics.OverlapSphere(sphereCenter, sphereRadius);
        if (overlappedColliders != null && overlappedColliders.Length != 0)
        {
            // Loop through each game object which is overlapped by the sphere and add it to the output list
            var overlappedObjects = new List<GameObject>(overlappedColliders.Length);
            foreach (Collider collider in overlappedColliders)
            {
                overlappedObjects.Add(collider.gameObject);
            }

            return overlappedObjects;
        }
        else return new List<GameObject>(); // If no object was overlpped, we will return an empty list
    }
    #endregion
}
