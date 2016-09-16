using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RTEditor
{
    /// <summary>
    /// Implements the behaviour for the runtime editor application. You can add functionality
    /// to this class to suit your own needs.
    /// </summary>
    [Serializable]
    public class RuntimeEditorApplication : MonoSingletonBase<RuntimeEditorApplication>
    {
        #region Private Variables
        /// <summary>
        /// If this is checked, the application will display some useful hints when running
        /// in play mode (only inside the Unity Editor).
        /// </summary>
        [SerializeField]
        private bool _showGUIHints = true;

        /// <summary>
        /// These are the settings which relate to any actions that need to be performed on
        /// application startup.
        /// </summary>
        [SerializeField]
        private RuntimeEditorApplicationStartupSettings _editorApplicationStartupSettings = new RuntimeEditorApplicationStartupSettings();
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets/sets the boolean flag which specifies if GUI hints must be shown when running
        /// in play mode.
        /// </summary>
        public bool ShowGUIHints { get { return _showGUIHints; } set { _showGUIHints = value; } }

        /// <summary>
        /// Returns the the settings which relate to any actions that need to be performed on
        /// application startup.
        /// </summary>
        public RuntimeEditorApplicationStartupSettings StartupSettings { get { return _editorApplicationStartupSettings; } }
        #endregion

        #region Public Methods
        /// <summary>
        /// Allows us to render any necessary GUI elements. You can delete this method
        /// or modify it to suit your own needs.
        /// </summary>
        public void OnGUI()
        {
            if(Application.isEditor && ShowGUIHints)
            {
                TransformPivotPoint transformPivotPoint = EditorGizmoSystem.Instance.TransformPivotPoint;
                TransformSpace transformSpace = EditorGizmoSystem.Instance.TransformSpace;

                GUI.color = Color.white;
                GUI.Box(new Rect(5, 5, 590, 170), "");
          
                GUI.Label(new Rect(10, 10, 300, 100), "Transform Pivot Point: " + transformPivotPoint.ToString() + " (P)");
                GUI.Label(new Rect(10, 30, 300, 100), "Transform Space: " + transformSpace.ToString() + " (G - global; L- local)");
                GUI.Label(new Rect(10, 50, 800, 100), "Gizmo keys: W - move gizmo; E - rotation gizmo; R - scale gizmo.");

                GUI.Label(new Rect(10, 70, 800, 100), "WASD, QE + right mouse button - move camera");
                GUI.Label(new Rect(10, 90, 800, 100), "F - focus camera on selection");
                GUI.Label(new Rect(10, 110, 800, 100), "ALT + mouse move + right mouse down - orbit around focus point (only after camera was focused)");

                GUI.color = Color.yellow;
                GUI.Label(new Rect(10, 130, 500, 100), "Undo: CTRL/CMD + SHIFT + Z (Unity Editor) and CTRL/CMD + Z (Build)");
                GUI.Label(new Rect(10, 150, 500, 100), "Redo: CTRL/CMD + SHIFT + Y (Unity Editor) and CTRL/CMD + Y (Build)");
            }
        }
        #endregion

        #if UNITY_EDITOR
        #region Menu Items
        /// <summary>
        /// Creates all the necessary subsystems which are needed for the runtime editor.
        /// </summary>
        [MenuItem("Runtime Editor Application/Create Subsystems")]
        private static void CreateSubsystems()
        {
            CreateRuntimeEditorApplicationSubsystems();
        }
        #endregion
        #endif


        #region Private Static Functions
        #if UNITY_EDITOR
        /// <summary>
        /// Creates all the necessary runtime editor subsystems.
        /// </summary>
        private static void CreateRuntimeEditorApplicationSubsystems()
        {
            // First, make sure all existing subsystems are destroyed
            DestroyExistingSubsystems();

            // Now, create each subsystem  
            RuntimeEditorApplication runtimeEditorApplication = RuntimeEditorApplication.Instance;
            Transform runtimeEditorApplicationTransform = runtimeEditorApplication.transform;

            EditorGizmoSystem editorGizmoSystem = EditorGizmoSystem.Instance;
            editorGizmoSystem.transform.parent = runtimeEditorApplicationTransform;

            EditorObjectSelection editorObjectSelection = EditorObjectSelection.Instance;
            editorObjectSelection.transform.parent = runtimeEditorApplicationTransform;

            EditorCamera editorCamera = EditorCamera.Instance;
            editorCamera.transform.parent = runtimeEditorApplicationTransform;
            editorCamera.gameObject.AddComponent<Camera>();

            EditorUndoRedoSystem editorUndoRedoSystem = EditorUndoRedoSystem.Instance;
            editorUndoRedoSystem.transform.parent = runtimeEditorApplicationTransform;

            EditorShortuctKeys editorShortcutKeys = EditorShortuctKeys.Instance;
            editorShortcutKeys.transform.parent = runtimeEditorApplicationTransform;

            // Create all gizmos and attach them to the gizmo system
            GameObject gizmoObject = new GameObject();
            gizmoObject.name = "Translation Gizmo";
            TranslationGizmo translationGizmo = gizmoObject.AddComponent<TranslationGizmo>();
            editorGizmoSystem.TranslationGizmo = translationGizmo;

            gizmoObject = new GameObject();
            gizmoObject.name = "Rotation Gizmo";
            RotationGizmo rotationGizmo = gizmoObject.AddComponent<RotationGizmo>();
            editorGizmoSystem.RotationGizmo = rotationGizmo;

            gizmoObject = new GameObject();
            gizmoObject.name = "Scale Gizmo";
            ScaleGizmo scaleGizmo = gizmoObject.AddComponent<ScaleGizmo>();
            editorGizmoSystem.ScaleGizmo = scaleGizmo;
        }

        /// <summary>
        /// Destroys all existing editor subsystems.
        /// </summary>
        private static void DestroyExistingSubsystems()
        {
            DestroyAllEntities(FindObjectsOfType<RuntimeEditorApplication>());
            DestroyAllEntities(FindObjectsOfType<EditorGizmoSystem>());
            DestroyAllEntities(FindObjectsOfType<EditorObjectSelection>());
            DestroyAllEntities(FindObjectsOfType<EditorCamera>());
            DestroyAllEntities(FindObjectsOfType<EditorUndoRedoSystem>());
            DestroyAllEntities(FindObjectsOfType<EditorShortuctKeys>());
            DestroyAllEntities(FindObjectsOfType<TranslationGizmo>());
            DestroyAllEntities(FindObjectsOfType<RotationGizmo>());
            DestroyAllEntities(FindObjectsOfType<ScaleGizmo>());
        }

        /// <summary>
        /// This function recieves a list of entities whose type must derive from 'MonoBehaviour'
        /// and destorys their associated game objects.
        /// </summary>
        private static void DestroyAllEntities<DataType>(DataType[] entitiesToDestroy) where DataType : MonoBehaviour
        {
            foreach (DataType entity in entitiesToDestroy)
            {
                DestroyImmediate(entity.gameObject);
            }
        }
        #endif
        #endregion

        #region Private Methods
        /// <summary>
        /// Performs any necessary initializations.
        /// </summary>
        private void Start()
        {
            OnStartup();
        }

        /// <summary>
        /// Called to perform any necessary initializations.
        /// </summary>
        private void OnStartup()
        {
            // If necessary, create the mesh vertex group mappings for vertex snpping
            if (_editorApplicationStartupSettings.AcquireVertexSnappingInfoOnStartup) MeshVertexGroupMappings.Instance.CreateMappingsForAllSceneMeshObjects();

            // Attach any colliders if necessary
            if (_editorApplicationStartupSettings.AttachObjectCollidersToAllSceneObjectsAtStartup)
                ObjectColliderAttachment.Instance.AttachCollidersToAllSceneObjects(_editorApplicationStartupSettings.ObjectColliderAttachmentSettings);
        }
        #endregion
    }
}
