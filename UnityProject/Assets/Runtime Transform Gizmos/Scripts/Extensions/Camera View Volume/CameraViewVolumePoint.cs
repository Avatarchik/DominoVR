namespace RTEditor
{
    /// <summary>
    /// This enum can be used to identify different camera view volume points. The member names
    /// were given such that the points can be identified when looking down the camera look
    /// vector.
    /// </summary>
    public enum CameraViewVolumePoint
    {
        /// <summary>
        /// The top left point on the camera's near plane.
        /// </summary>
        TopLeftOnNearPlane = 0,

        /// <summary>
        /// The top right point on the camera's near plane.
        /// </summary>
        TopRightOnNearPlane,

        /// <summary>
        /// The bottom right point on the camera's near plane.
        /// </summary>
        BottomRightOnNearPlane,

        /// <summary>
        /// The bottom left point on the camera's near plane.
        /// </summary>
        BottomLeftOnNearPlane,

        /// <summary>
        /// The top left point on the camera's far plane.
        /// </summary>
        TopLeftOnFarPlane,

        /// <summary>
        /// The top right point on the camera's far plane.
        /// </summary>
        TopRightOnFarPlane,

        /// <summary>
        /// The bottom right point on the camera's far plane.
        /// </summary>
        BottomRightOnFarPlane,

        /// <summary>
        /// The bottom left point on the camera's far plane.
        /// </summary>
        BottomLeftOnFarPlane
    }
}
