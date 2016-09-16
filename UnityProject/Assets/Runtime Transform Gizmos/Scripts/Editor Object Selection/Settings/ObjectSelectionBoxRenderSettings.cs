using UnityEngine;
using System;

namespace RTEditor
{
    /// <summary>
    /// Holds settings which control the way in which an object selection box is rendered.
    /// </summary>
    [Serializable]
    public class ObjectSelectionBoxRenderSettings
    {
        #region Private Variables
        /// <summary>
        /// The selection box style.
        /// </summary>
        [SerializeField]
        private ObjectSelectionBoxStyle _selectionBoxStyle = ObjectSelectionBoxStyle.CornerLines;

        /// <summary>
        /// When '_selectionBoxStyle' is set to 'CornerLines', this value controls the length of 
        /// the selection box lines which meet at a corner.
        /// </summary>
        [SerializeField]
        private float _selectionBoxCornerLineLength = 0.4f;

        /// <summary>
        /// The color which must be used to render the object selection box lines.
        /// </summary>
        [SerializeField]
        private Color _selectionBoxLineColor = new Color(1.0f, 1.0f, 1.0f, 0.53f);

        /// <summary>
        /// The scale factor which must be applied to the selection boxes when they are rendered.
        /// </summary>
        [SerializeField]
        private float _selectionBoxScaleFactor = 1.009f;
        #endregion

        #region Public Static Properties
        /// <summary>
        /// Returns the minimum value that the corner line length can have.
        /// </summary>
        public static float MinSelectionBoxCornerLineLength { get { return 0.01f; } }

        /// <summary>
        /// Returns the minimum value that the selection box scale box factor can have.
        /// </summary>
        public static float MinSelectionBoxScaleFactor { get { return 0.01f; } }
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets/sets the object selection box style.
        /// </summary>
        public ObjectSelectionBoxStyle SelectionBoxStyle { get { return _selectionBoxStyle; } set { _selectionBoxStyle = value; } }

        /// <summary>
        /// Gets/sets the selection box corner line length. The minimum value that the corner line
        /// length can have is given by the 'MinSelectionBoxCornerLineLength' property. Values 
        /// smaller than that will be clamped acordingly.
        /// </summary>
        public float SelectionBoxCornerLineLength { get { return _selectionBoxCornerLineLength; } set { _selectionBoxCornerLineLength = Mathf.Max(MinSelectionBoxCornerLineLength, value); } }

        /// <summary>
        /// Gets/sets the object selection box color.
        /// </summary>
        public Color SelectionBoxLineColor { get { return _selectionBoxLineColor; } set { _selectionBoxLineColor = value; } }

        /// <summary>
        /// Gets/sets the scale factor that must be used when a selection box is rendered. The minimum
        /// value that the scale factor can have is given by the 'MinSelectionBoxScaleFactor' property. 
        /// Values smaller than that will be clamped accordingly.
        /// </summary>
        public float SelectionBoxScaleFactor { get { return _selectionBoxScaleFactor; } set { _selectionBoxScaleFactor = Mathf.Max(MinSelectionBoxScaleFactor, value); } }
        #endregion
    }
}
