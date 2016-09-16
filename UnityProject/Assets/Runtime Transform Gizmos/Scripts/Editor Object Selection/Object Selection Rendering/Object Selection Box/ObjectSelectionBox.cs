using UnityEngine;

namespace RTEditor
{
    /// <summary>
    /// Holds information for an object selection box.
    /// </summary>
    public class ObjectSelectionBox
    {
        #region Private Variables
        /// <summary>
        /// The object selection box in its untransformed/model space.
        /// </summary>
        private Bounds _modelSpaceBox;

        /// <summary>
        /// This is the transform matrix associated with the selection box. This is
        /// useful when we need to render selection boxes because it allows us to
        /// transform the model space box as needed.
        /// </summary>
        private Matrix4x4 _transformMatrix;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ObjectSelectionBox()
        {
            _modelSpaceBox = BoundsExtensions.GetInvalidBoundsInstance();
            _transformMatrix = Matrix4x4.identity;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="modelSpaceBox">
        /// The object selection box in its untransformed/model space.
        /// </param>
        public ObjectSelectionBox(Bounds modelSpaceBox)
        {
            _modelSpaceBox = modelSpaceBox;
            _transformMatrix = Matrix4x4.identity;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="modelSpaceBox">
        /// The object selection box in its untransformed/model space.
        /// </param>
        /// <param name="transformMatrix">
        /// The transform matrix associated with the object selection box. 
        /// </param>
        public ObjectSelectionBox(Bounds modelSpaceBox, Matrix4x4 transformMatrix)
        {
            _modelSpaceBox = modelSpaceBox;
            _transformMatrix = transformMatrix;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets/sets the model space selection box.
        /// </summary>
        public Bounds ModelSpaceBox { get { return _modelSpaceBox; } set { _modelSpaceBox = value; } }

        /// <summary>
        /// Gets/sets the selection box transform matrix.
        /// </summary>
        public Matrix4x4 TransformMatrix { get { return _transformMatrix; } set { _transformMatrix = value; } }
        #endregion
    }
}
