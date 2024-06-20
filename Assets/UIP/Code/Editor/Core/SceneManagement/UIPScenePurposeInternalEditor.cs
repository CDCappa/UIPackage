using System;

using UIP.Runtime.Core.SceneManagement;

using UnityEditor;

using UnityEngine;

namespace UIP.Editor.Core.SceneManagement
{
    [CustomEditor(typeof(UIPScenePurposeInternal))]
    public class UIPScenePurposeInternalEditor : UnityEditor.Editor
    {
        protected new UIPScenePurposeInternal target;

        private bool _imUIPDeveloper = false;
        private bool _editable = false;
        private Texture2D _warningIcon;
        private const float WARNING_ICON_SIZE = 16f;

        private const string VANILLA_WARNING_ICON_PATH = "icons/console.warnicon.sml.png";
        private const string SCENE_PURPOSE_HEADER = "SCENE PURPOSE";
        private const string SCENE_ASSET_HEADER = "ESCENE ASSET";
        private const string AUTOMATIC_TAG = "AUTOMATIC";
        private const string WARNING_TEXT = "The core of UIP relies on this standard configuration. If you edit this, you do so at your own risk.";
        private const string IM_UIP_DEVELOPER_TEXT = "I'm UIP developer";

        protected virtual void OnEnable()
        {
            this.target = (UIPScenePurposeInternal)base.target;
            _editable = this is ScenePurposeConfigurationEditor;
            _warningIcon = EditorGUIUtility.Load(VANILLA_WARNING_ICON_PATH) as Texture2D;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
            style.normal.textColor = Color.red;

            GUILayout.BeginHorizontal();

            GUILayout.Label(SCENE_PURPOSE_HEADER, style);
            GUILayout.Label(SCENE_ASSET_HEADER, style);
            GUILayout.EndHorizontal();

            if (!_editable && !_imUIPDeveloper)
            {
                EditorGUI.BeginDisabledGroup(true);
            }

            style.normal.textColor = Color.white;
            foreach (ScenePurpose purpose in Enum.GetValues(typeof(ScenePurpose)))
            {
                GUILayout.BeginHorizontal();

                if (_editable)
                {
                    if (purpose.Equals(ScenePurpose.STARTUP))
                    {
                        EditorGUILayout.LabelField(purpose.ToString(), style);
                        style.normal.textColor = EditorStyles.label.normal.textColor;
                        EditorGUILayout.LabelField(AUTOMATIC_TAG);
                        style.normal.textColor = Color.white;
                    }
                    else if (!purpose.Equals(ScenePurpose.NONE))
                    {
                        EditorGUILayout.LabelField(purpose.ToString(), style);
                        target.SetSceneAsset(purpose, (SceneAsset)EditorGUILayout.ObjectField(target.GetScene(purpose).Asset, typeof(SceneAsset), false));
                    }
                }
                else
                {
                    EditorGUILayout.LabelField(purpose.ToString(), style);
                    target.SetSceneAsset(purpose, (SceneAsset)EditorGUILayout.ObjectField(target.GetScene(purpose).Asset, typeof(SceneAsset), false));
                }

                GUILayout.EndHorizontal();
            }

            if (!_editable)
            {
                EditorGUI.EndDisabledGroup();
                GUILayout.BeginHorizontal();
                GUIContent warningContent = new GUIContent(_warningIcon, WARNING_TEXT);
                GUILayout.Label(warningContent, GUILayout.Width(WARNING_ICON_SIZE), GUILayout.Height(WARNING_ICON_SIZE));
                _imUIPDeveloper = EditorGUILayout.Toggle(IM_UIP_DEVELOPER_TEXT, _imUIPDeveloper);
                GUILayout.EndHorizontal();
            }

            serializedObject.ApplyModifiedProperties();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(target);
            }
        }
    }
}