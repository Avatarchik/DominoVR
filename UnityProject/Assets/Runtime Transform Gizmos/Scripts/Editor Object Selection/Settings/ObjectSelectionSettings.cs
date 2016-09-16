using UnityEngine;
using System;

namespace RTEditor
{
    /// <summary>
    /// This class holds all object selection related settings.
    /// </summary>
    [Serializable]
    public class ObjectSelectionSettings
    {
        #region Private Variables
        /// <summary>
        /// The object selection mode which controls how objects are selected.
        /// </summary>
        [SerializeField]
        private ObjectSelectionMode _objectSelectionMode = ObjectSelectionMode.IndividualObjects;

        /// <summary>
        /// The object selection render mode which controls the way in which the object 
        /// selection is rendered.
        /// </summary>
        [SerializeField]
        private ObjectSelectionRenderMode _objectSelectionRenderMode = ObjectSelectionRenderMode.SelectionBoxes;

        /// <summary>
        /// If this is set to true, any selected objects will be deselected when the selection mechanism
        /// is disabled.
        /// </summary>
        [SerializeField]
        private bool _deselectObjectsWhenDisabled = false;

        /// <summary>
        /// Only objects that belong to these layers can be selected.
        /// </summary>
        [SerializeField]
        private int _selectableLayers = ~0;

        /// <summary>
        /// Holds the object selection box render settings. These settings are used when the object selection
        /// render mode is set to 'SelectionBoxes'.
        /// </summary>
        [SerializeField]
        private ObjectSelectionBoxRenderSettings _objectSelectionBoxRenderSettings = new ObjectSelectionBoxRenderSettings();
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets/sets the object selection mode.
        /// </summary>
        public ObjectSelectionMode ObjectSelectionMode { get { return _objectSelectionMode; } set { _objectSelectionMode = value; } }

        /// <summary>
        /// Gets/sets the object selection render mode.
        /// </summary>
        public ObjectSelectionRenderMode ObjectSelectionRenderMode { get { return _objectSelectionRenderMode; } set { _objectSelectionRenderMode = value; } }

        /// <summary>
        /// Gets/sets the boolean flag which specifies whether or not any selected objects must be deselected
        /// when the selection mechanism is disabled.
        /// </summary>
        public bool DeselectObjectsWhenDisabled { get { return _deselectObjectsWhenDisabled; } set { _deselectObjectsWhenDisabled = value; } }

        /// <summary>
        /// Gets/sets the selectable layers.
        /// </summary>
        public int SelectableLayers { get { return _selectableLayers; } set { _selectableLayers = value; } }

        /// <summary>
        /// Returns the object selection box render settings.
        /// </summary>
        public ObjectSelectionBoxRenderSettings ObjectSelectionBoxRenderSettings { get { return _objectSelectionBoxRenderSettings; } }
        #endregion
    }
}
