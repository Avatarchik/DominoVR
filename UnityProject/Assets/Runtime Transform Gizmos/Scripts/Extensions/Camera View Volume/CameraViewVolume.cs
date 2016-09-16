using UnityEngine;
using System.Collections.Generic;

namespace RTEditor
{
    /// <summary>
    /// This class represents a camera view volume. The view volume differs based on the
    /// type of camera we are dealing with. For an orthographic camera, the view volume
    /// is a box. For a perspective camera, the view volume is a frustum.
    /// </summary>
    public class CameraViewVolume
    {
        #region Private Variables
        /// <summary>
        /// Holds the camera view volume points in world space. This array can be accessed
        /// using members of the 'CameraViewVolumePoint' as indices.
        /// </summary>
        private Vector3[] _worldSpaceVolumePoints = new Vector3[8];

        /// <summary>
        /// This array holds the view volume's planes in world space. The elements in this array 
        /// can be accessed using members of the 'CameraViewVolumePlane' enum as indices. The 
        /// planes are stored in the following order: left, right, bottom, top, near, far. All
        /// planes are pointing inside the volume.
        /// </summary>
        private Plane[] _worldSpacePlanes = new Plane[6];
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns a copy of the internal world space volume array. This list can be accessed
        /// using members of the 'CameraViewVolumePoint' as indices.
        /// </summary>
        public Vector3[] WorldSpaceVolumePoints { get { return _worldSpaceVolumePoints.Clone() as Vector3[]; } }

        /// <summary>
        /// Returns the top left point which resides on the camera near plane.
        /// </summary>
        public Vector3 TopLeftPointOnNearPlane { get { return _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopLeftOnNearPlane]; } }

        /// <summary>
        /// Returns the top right point which resides on the camera near plane.
        /// </summary>
        public Vector3 TopRightPointOnNearPlane { get { return _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopRightOnNearPlane]; } }

        /// <summary>
        /// Returns the bottom right point which resides on the camera near plane.
        /// </summary>
        public Vector3 BottomRightPointOnNearPlane { get { return _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomRightOnNearPlane]; } }

        /// <summary>
        /// Returns the bottom left point which resides on the camera near plane.
        /// </summary>
        public Vector3 BottomLeftPointOnNearPlane { get { return _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomLeftOnNearPlane]; } }

        /// <summary>
        /// Returns the top left point which resides on the camera far plane.
        /// </summary>
        public Vector3 TopLeftPointOnFarPlane { get { return _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopLeftOnFarPlane]; } }

        /// <summary>
        /// Returns the top right point which resides on the camera far plane.
        /// </summary>
        public Vector3 TopRightPointOnFarPlane { get { return _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopRightOnFarPlane]; } }

        /// <summary>
        /// Returns the bottom right point which resides on the camera far plane.
        /// </summary>
        public Vector3 BottomRightPointOnFarPlane { get { return _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomRightOnFarPlane]; } }

        /// <summary>
        /// Returns the bottom left point which resides on the camera far plane.
        /// </summary>
        public Vector3 BottomLeftPointOnFarPlane { get { return _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomLeftOnFarPlane]; } }

        /// <summary>
        /// Returns the volume's world space planes. The elements in this array can be accessed 
        /// using members of the 'CameraViewVolumePlane' enum as indices. The planes are stored 
        /// in the following order: left, right, bottom, top, near, far. All planes are pointing 
        /// inside the volume.
        /// </summary>
        public Plane[] WorldSpacePlanes { get { return _worldSpacePlanes.Clone() as Plane[]; } }

        /// <summary>
        /// Returns the volume's left plane. The plane is pointing inside the view volume.
        /// </summary>
        public Plane LeftPlane { get { return _worldSpacePlanes[(int)CameraViewVolumePlane.Left]; } }

        /// <summary>
        /// Returns the volume's right plane. The plane is pointing inside the view volume.
        /// </summary>
        public Plane RightPlane { get { return _worldSpacePlanes[(int)CameraViewVolumePlane.Right]; } }

        /// <summary>
        /// Returns the volume's bottom plane. The plane is pointing inside the view volume.
        /// </summary>
        public Plane BottomPlane { get { return _worldSpacePlanes[(int)CameraViewVolumePlane.Bottom]; } }

        /// <summary>
        /// Returns the volume's top plane. The plane is pointing inside the view volume.
        /// </summary>
        public Plane TopPlane { get { return _worldSpacePlanes[(int)CameraViewVolumePlane.Top]; } }

        /// <summary>
        /// Returns the volume's near plane. The plane is pointing inside the view volume.
        /// </summary>
        public Plane NearPlane { get { return _worldSpacePlanes[(int)CameraViewVolumePlane.Near]; } }

        /// <summary>
        /// Returns the volume's far plane. The plane is pointing inside the view volume.
        /// </summary>
        public Plane FarPlane { get { return _worldSpacePlanes[(int)CameraViewVolumePlane.Far]; } }
        #endregion

        #region Public Methods
        /// <summary>
        /// Builds the view volume for the specified camera. You should call this method
        /// before accessing any properties of this class.
        /// </summary>
        public void BuildForCamera(Camera camera)
        {
            IdentifyVolumeWorldSpacePointsForCamera(camera);
            CalculateWorldSpacePlanes(camera);
        }

        /// <summary>
        /// Checks if the volume contains (either fully or partially) the specified world space AABB.
        /// </summary>
        /// <remarks>
        /// Call 'BuildForCamera' before calling this method in order to make sure that the volume
        /// contains the correct data.
        /// </remarks>
        public bool ContainsWorldSpaceAABB(Bounds worldSpaceAABB)
        {
            return GeometryUtility.TestPlanesAABB(_worldSpacePlanes, worldSpaceAABB);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Identifies the volume world space points for the specified camera.
        /// </summary>
        private void IdentifyVolumeWorldSpacePointsForCamera(Camera camera)
        {
            if (camera == null) return;

            // Cache needed data for easy access
            Transform cameraTransform = camera.transform;
            Vector3 cameraPosition = cameraTransform.position;
            Vector3 cameraRight = cameraTransform.right;
            Vector3 cameraUp = cameraTransform.up;
            Vector3 cameraLook = cameraTransform.forward;

            // Identify the volume points based on the camera we are dealing with
            if (camera.orthographic)
            {
                // Calculate the halfs of the view volume size.
                // Note: The horizontal size is calculated by taking into account the aspect ratio.
                float halfVolumeVerticalSize = camera.orthographicSize;
                float halfVolumeHorizontalSize = halfVolumeVerticalSize * camera.aspect;

                // Calculate the volume points on the near plane
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopLeftOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane - cameraRight * halfVolumeHorizontalSize + cameraUp * halfVolumeVerticalSize;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopRightOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane + cameraRight * halfVolumeHorizontalSize + cameraUp * halfVolumeVerticalSize;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomRightOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane + cameraRight * halfVolumeHorizontalSize - cameraUp * halfVolumeVerticalSize;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomLeftOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane - cameraRight * halfVolumeHorizontalSize - cameraUp * halfVolumeVerticalSize;

                // Calculate the volume points on the far plane
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopLeftOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane - cameraRight * halfVolumeHorizontalSize + cameraUp * halfVolumeVerticalSize;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopRightOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane - cameraRight * halfVolumeHorizontalSize + cameraUp * halfVolumeVerticalSize;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomRightOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane - cameraRight * halfVolumeHorizontalSize + cameraUp * halfVolumeVerticalSize;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomLeftOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane - cameraRight * halfVolumeHorizontalSize + cameraUp * halfVolumeVerticalSize;
            }
            else
            {
                // In order to identify the volume world space points, we will need to know how much
                // we have to travel along the X and Y axes for one unit of movement along Z. Because
                // the view volume for a perspective camera is a frustum, we will need to calculate the
                // ratios of movement using the tangent of the camera's half field of view angle.
                float halfFovAngle = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
                float angleTangent = Mathf.Tan(halfFovAngle);

                // Use the tangent to calculate the move ratios.
                // Note: For 'xToZRatio', we have to multiply by the camera aspect ratio to account for screen distortion.
                float xToZRatio = angleTangent * camera.aspect;
                float yToZRatio = angleTangent;

                // We will use these variables to speed up the frustum point calculation
                float xRatioMulNearClipPlane = xToZRatio * camera.nearClipPlane;
                float xRatioMulFarClipPlane = xToZRatio * camera.farClipPlane;
                float yRatioMulNearClipPlane = yToZRatio * camera.nearClipPlane;
                float yRatioMulFarClipPlane = yToZRatio * camera.farClipPlane;

                // Calculate the volume points on the near plane
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopLeftOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane - cameraRight * xRatioMulNearClipPlane + cameraUp * yRatioMulNearClipPlane;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopRightOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane + cameraRight * xRatioMulNearClipPlane + cameraUp * yRatioMulNearClipPlane;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomRightOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane + cameraRight * xRatioMulNearClipPlane - cameraUp * yRatioMulNearClipPlane;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomLeftOnNearPlane] = cameraPosition + cameraLook * camera.nearClipPlane - cameraRight * xRatioMulNearClipPlane - cameraUp * yRatioMulNearClipPlane;

                // Calculate the volume points on the far plane
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopLeftOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane - cameraRight * xRatioMulFarClipPlane + cameraUp * yRatioMulFarClipPlane;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.TopRightOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane + cameraRight * xRatioMulFarClipPlane + cameraUp * yRatioMulFarClipPlane;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomRightOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane + cameraRight * xRatioMulFarClipPlane - cameraUp * yRatioMulFarClipPlane;
                _worldSpaceVolumePoints[(int)CameraViewVolumePoint.BottomLeftOnFarPlane] = cameraPosition + cameraLook * camera.farClipPlane - cameraRight * xRatioMulFarClipPlane - cameraUp * yRatioMulFarClipPlane;
            }
        }

        /// <summary>
        /// Calculates the volume's world space planes for the specified camera.
        /// </summary>
        private void CalculateWorldSpacePlanes(Camera camera)
        {
            // Use 'GeometryUtility.CalculateFrustumPlanes' for now. This will probably be changed in the future.
            _worldSpacePlanes = GeometryUtility.CalculateFrustumPlanes(camera);
        }
        #endregion
    }
}
