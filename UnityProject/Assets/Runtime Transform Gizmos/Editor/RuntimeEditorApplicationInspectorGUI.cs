using UnityEngine;
using UnityEditor;
using System;

namespace RTEditor
{
    /// <summary>
    /// Custom inspector implementation for the 'EditorApplication' class.
    /// </summary>
    [CustomEditor(typeof(RuntimeEditorApplication))]
    public class RuntimeEditorApplicationInspectorGUI : Editor
    {
        #region Private Variables
        /// <summary>
        /// Reference to the currently selected editor application object.
        /// </summary>
        private RuntimeEditorApplication _editorApplication;

        /// <summary>
        /// The following variables control the visibility for different categories of settings.
        /// </summary>
        private static bool _startupSettingsAreVisible = true;
        private static bool _objectColliderAttachmentSettingsAreVisible = true;
        #endregion

        #region Public Methods
        /// <summary>
        /// Called when the inspector needs to be rendered.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Let the user specify if GUI hints must be shown
            bool newBooleanValue = EditorGUILayout.ToggleLeft("Show GUI hints", _editorApplication.ShowGUIHints);
            if(newBooleanValue != _editorApplication.ShowGUIHints)
            {
                UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);
                _editorApplication.ShowGUIHints = newBooleanValue;
            }

            _startupSettingsAreVisible = EditorGUILayout.Foldout(_startupSettingsAreVisible, "Startup Settings");
            if(_startupSettingsAreVisible)
            {
                RuntimeEditorApplicationStartupSettings startupSettings = _editorApplication.StartupSettings;

                // Let the user specify whether or not vertex snapping info must be acquired at startup
                newBooleanValue = EditorGUILayout.ToggleLeft("Acquire Vertex Snapping Info At Startup", startupSettings.AcquireVertexSnappingInfoOnStartup);
                if(newBooleanValue != startupSettings.AcquireVertexSnappingInfoOnStartup)
                {
                    UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);
                    startupSettings.AcquireVertexSnappingInfoOnStartup = newBooleanValue;
                }

                // Let the user specify object collider attachment settings
                const int indentLevel = 1;
                EditorGUI.indentLevel += indentLevel;
                _objectColliderAttachmentSettingsAreVisible = EditorGUILayout.Foldout(_objectColliderAttachmentSettingsAreVisible, "Object Collider Attachment Settings");
                if(_objectColliderAttachmentSettingsAreVisible)
                {
                    // Let the user specify whether or not object colliders must be attached to all objects at startup
                    newBooleanValue = EditorGUILayout.ToggleLeft("Attach Colliders To All Scene Objects At Startup", startupSettings.AttachObjectCollidersToAllSceneObjectsAtStartup);
                    if(newBooleanValue != startupSettings.AttachObjectCollidersToAllSceneObjectsAtStartup)
                    {
                        UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);
                        startupSettings.AttachObjectCollidersToAllSceneObjectsAtStartup = newBooleanValue;
                    }

                    // If object colliders must be attached at startup, let the user modify the attachment settings
                    if(startupSettings.AttachObjectCollidersToAllSceneObjectsAtStartup)
                    {
                        ObjectColliderAttachmentSettings colliderAttachmentSettings = startupSettings.ObjectColliderAttachmentSettings;

                        // Let the user specify which object types should be ignored during the collider attachment process
                        EditorGUILayout.Separator();
                        newBooleanValue = EditorGUILayout.ToggleLeft("Ignore Mesh Objects", startupSettings.ObjectColliderAttachmentSettings.IgnoreMeshObjects);
                        if (newBooleanValue != startupSettings.ObjectColliderAttachmentSettings.IgnoreMeshObjects)
                        {
                            UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);
                            startupSettings.ObjectColliderAttachmentSettings.IgnoreMeshObjects = newBooleanValue;
                        }

                        newBooleanValue = EditorGUILayout.ToggleLeft("Ignore Light Objects", startupSettings.ObjectColliderAttachmentSettings.IgnoreLightObjects);
                        if (newBooleanValue != startupSettings.ObjectColliderAttachmentSettings.IgnoreLightObjects)
                        {
                            UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);
                            startupSettings.ObjectColliderAttachmentSettings.IgnoreLightObjects = newBooleanValue;
                        }

                        newBooleanValue = EditorGUILayout.ToggleLeft("Ignore Particle System Objects", startupSettings.ObjectColliderAttachmentSettings.IgnoreParticleSystemObjects);
                        if (newBooleanValue != startupSettings.ObjectColliderAttachmentSettings.IgnoreParticleSystemObjects)
                        {
                            UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);
                            startupSettings.ObjectColliderAttachmentSettings.IgnoreParticleSystemObjects = newBooleanValue;
                        }

                        // Let the user specify the type of collider which must be attched to mesh objects
                        EditorGUILayout.Separator();
                        ObjectColliderType newObjectColliderType = (ObjectColliderType)EditorGUILayout.EnumPopup("Collider Type For Mesh Objects", colliderAttachmentSettings.ColliderTypeForMeshObjects);
                        if(newObjectColliderType != colliderAttachmentSettings.ColliderTypeForMeshObjects)
                        {
                            UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);
                            colliderAttachmentSettings.ColliderTypeForMeshObjects = newObjectColliderType;
                        }

                        // Let the user specify the type of collider which must be attched to light objects
                        string[] nonMeshObjectColliderTypes = ObjectColliderTypeHelper.GetPossibleObjectColliderTypeNames(new ObjectColliderType[] { ObjectColliderType.MeshCollider });
                        int selectedIndex = Array.IndexOf(nonMeshObjectColliderTypes, colliderAttachmentSettings.ColliderTypeForLightObjects.ToString());
                        int newInt = EditorGUILayout.Popup("Collider Type For Light Objects", selectedIndex, nonMeshObjectColliderTypes);
                        if (newInt != selectedIndex)
                        {
                            UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);

                            ObjectColliderTypeHelper.GetObjectColliderTypeByName(nonMeshObjectColliderTypes[newInt], out newObjectColliderType);
                            colliderAttachmentSettings.ColliderTypeForLightObjects = newObjectColliderType;
                        }

                        // Let the user specify the type of collider which must be attched to particle system objects
                        selectedIndex = Array.IndexOf(nonMeshObjectColliderTypes, colliderAttachmentSettings.ColliderTypeForParticleSystemObjects.ToString());
                        newInt = EditorGUILayout.Popup("Collider Type For Particle System Objects", selectedIndex, nonMeshObjectColliderTypes);
                        if (newInt != selectedIndex)
                        {
                            UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);

                            ObjectColliderTypeHelper.GetObjectColliderTypeByName(nonMeshObjectColliderTypes[newInt], out newObjectColliderType);
                            colliderAttachmentSettings.ColliderTypeForParticleSystemObjects = newObjectColliderType;
                        }

                        // Let the user specify the box collider size for non-mesh objects
                        EditorGUILayout.Separator();
                        Vector3 newVectorValue = EditorGUILayout.Vector3Field("Box Collider Size For Non-Mesh Objects", colliderAttachmentSettings.BoxColliderSizeForNonMeshObjects);
                        if(newVectorValue != colliderAttachmentSettings.BoxColliderSizeForNonMeshObjects)
                        {
                            UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);
                            colliderAttachmentSettings.BoxColliderSizeForNonMeshObjects = newVectorValue;
                        }

                        // Let the user specify the sphere collider radius for non-mesh objects
                        float newFloatValue = EditorGUILayout.FloatField("Sphere Collider Radius For Non-Mesh Objects", colliderAttachmentSettings.SphereColliderRadiusForNonMeshObjects);
                        if(newFloatValue != colliderAttachmentSettings.SphereColliderRadiusForNonMeshObjects)
                        {
                            UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);
                            colliderAttachmentSettings.SphereColliderRadiusForNonMeshObjects = newFloatValue;
                        }

                        // Let the user specify the capsule collider radius for non-mesh objects
                        newFloatValue = EditorGUILayout.FloatField("Capsule Collider Radius For Non-Mesh Objects", colliderAttachmentSettings.CapsuleColliderRadiusForNonMeshObjects);
                        if (newFloatValue != colliderAttachmentSettings.CapsuleColliderRadiusForNonMeshObjects)
                        {
                            UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);
                            colliderAttachmentSettings.CapsuleColliderRadiusForNonMeshObjects = newFloatValue;
                        }

                        // Let the user specify the capsule collider height for non-mesh objects
                        newFloatValue = EditorGUILayout.FloatField("Capsule Collider Height For Non-Mesh Objects", colliderAttachmentSettings.CapsuleColliderHeightForNonMeshObjects);
                        if (newFloatValue != colliderAttachmentSettings.CapsuleColliderHeightForNonMeshObjects)
                        {
                            UnityEditorUndoHelper.RecordObjectForInspectorPropertyChange(_editorApplication);
                            colliderAttachmentSettings.CapsuleColliderHeightForNonMeshObjects = newFloatValue;
                        }
                    }
                }
                EditorGUI.indentLevel -= indentLevel;
            }
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Called when the editor application object is selected in the scene view.
        /// </summary>
        protected virtual void OnEnable()
        {
            _editorApplication = target as RuntimeEditorApplication;
        }
        #endregion
    }
}
