using System.Collections.Generic;
using System.Linq;

using UIP.Runtime.Core.Initialization;
using UIP.Runtime.Core.SceneManagement;

using UnityEditor;

using UnityEngine;

namespace UIP.Editor.Core.SceneManagement
{
    [CustomEditor(typeof(ScenePurposeConfiguration))]
    public class ScenePurposeConfigurationEditor : UIPScenePurposeInternalEditor
    {
        [SerializeField] private static List<string> AllInstances;
        [SerializeField] private static List<string> _previousAllInstances;
        [SerializeField] private static string _previousSelectedFilePath;
        [SerializeField] private static string _currentSelectedFilePath;

        [SerializeField] [HideInInspector] private ScenePurposeConfiguration _userTarget;

        #region consts
        private const string EDITORPREFS_CURRENT_SELECTED_FILE_KEY = "SceneManagement-CurrentSelectedScenePurposeConfiguration";
        private const string EDITORPREFS_PREVIOUS_SELECTED_FILE_KEY = "SceneManagement-PreviousSelectedScenePurposeConfiguration";
        private const string EDITORPREFS_PREVIOUS_ALL_INSTANCES_KEY = "SceneManagement-PreviousAllScenePurposeConfigurationInstances";
        private const string PREVIOUS_ALL_INSTANCES_REMAP_SEPARATOR = ";";

        private const string WARNING_FILE_NOT_SELECTED_TEXT = "This configuration file is not selected for use in the UIP initialization.";
        private const string WARNING_CURRENTLY_FILE_SELECTED_TAG = "Currently selected file:";
        private const string WARNING_BUTTON_INSTRUCTIONS_TEXT = "If you want to initialize UIP with this file, click the button below.";

        private const string BUTTON_TEXT = "Select this file";

        private const string INFO_TEXT =
            "The objective of UIP is to provide a robust and automated UI and establish generic rules for a game or a group of games compatible with the same UI.\n\n" +
            "1. The flexibility is provided that the defined scenes can be replaced at runtime (e.g.: you can change the scene whose purpose is \"Game\" during runtime if you develop levels in different scenes).\n" +
            "2. The scene purpose \"Loading\" applies to all cases, whether it's the start of the game or transition from the main menu to the game for example.\n" +
            "3. It is not necessary for the game to have the same number of scenes as UIP. For example: if a level is composed of 3 scenes, declare only the first one of the series.\n" +
            "4. If you need more UI, create your own canvas in de game side; it won't interfere with the generic UIP canvas.\n" +
            "5. Leaving empty references in this configuration file does not break the system; it simply extends UI states.";

        private const string FILES_REPORT_HEADER = "Configuration Files (ScenePurposeConfiguration) present in the project:";
        private const string FILES_REPORT_CURRENT_SELECTED_TAG = "Current selected:";
        private const string FILES_REPORT_PREVIOUS_SELECTED_TAG = "Previous selected:";
        private const string FILES_REPORT_IDLE_FILES_LIST_HEADER = "Idle Files:";
        private const string FILES_REPORT_LIST_DOT = "-";

        private const string THIS_FILE_SUFIX = "(THIS FILE)";
        private const string DESTROYED_FILE_PREFIX = "DESTROYED FILE:";
        private const string NONE_FILE_NAME = "NONE";
        #endregion

        protected override void OnEnable()
        {
            base.OnEnable();
            Initialize();
        }

        private void Initialize()
        {
            _userTarget = (ScenePurposeConfiguration)target;
            AllInstances = new();
            _previousAllInstances = new();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!_userTarget.IsSelected)
            {
                GUI.backgroundColor = Color.yellow;

                EditorGUILayout.HelpBox(
                    $"{WARNING_FILE_NOT_SELECTED_TEXT}\n" +
                    $"{WARNING_CURRENTLY_FILE_SELECTED_TAG} {GetCurrentSelectedFilePath()}\n" +
                    WARNING_BUTTON_INSTRUCTIONS_TEXT,
                    MessageType.Warning
                );

                UpdateFileList(GUILayout.Button(BUTTON_TEXT));
            }
            else
            {
                UpdateFileList(buttonPressed: false);
            }

            GUI.backgroundColor = Color.white;

            EditorGUILayout.HelpBox(INFO_TEXT, MessageType.Info);

            CraftFilesReport();
        }

        private void UpdateFileList(bool buttonPressed)
        {
            if (buttonPressed)
            {
                SetPreviousAllInstances(AllInstances);
            }

            AllInstances.Clear();

            string[] guids = AssetDatabase.FindAssets($"t:{_userTarget.GetType().Name}");

            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                AllInstances.Add(path);

                if (buttonPressed)
                {
                    ScenePurposeConfiguration instance = AssetDatabase.LoadAssetAtPath<ScenePurposeConfiguration>(path);
                    instance.IsPreviousSelected = false;

                    if (instance != null)
                    {
                        if (instance != _userTarget)
                        {
                            if (instance.IsSelected)
                            {
                                SetPreviousSelectedFilePath(path);
                                instance.IsSelected = false;
                                instance.IsPreviousSelected = true;
                            }
                        }
                        else
                        {
                            RectifyNomenclaturesIfCurrentSelectedFileHasBeenDeleted();
                            SetCurrentSelectedFilePath(path);
                            _userTarget.IsSelected = true;
                            UpdateScenePurposeConfiguration(_userTarget);
                        }
                    }
                    EditorUtility.SetDirty(instance);
                }
            }
        }

        private void RectifyNomenclaturesIfCurrentSelectedFileHasBeenDeleted()
        {
            if (GetCurrentSelectedFilePath().Contains(NONE_FILE_NAME) && GetPreviousSelectedFilePath().Contains(DESTROYED_FILE_PREFIX))
            {
                SetPreviousSelectedFilePath(NONE_FILE_NAME);
            }
        }

        private void CraftFilesReport()
        {
            if (GetPreviousAllInstances() != null)
            {
                List<string> filesDeleted = GetPreviousAllInstances().Except(AllInstances).ToList();
                foreach (string file in filesDeleted)
                {
                    if (GetPreviousSelectedFilePath().Equals(file))
                    {
                        SetPreviousSelectedFilePath($" {DESTROYED_FILE_PREFIX} {GetPreviousSelectedFilePath()}");
                    }
                    else if (GetCurrentSelectedFilePath().Equals(file))
                    {
                        ScenePurposeConfiguration currentSelectedFile = AssetDatabase.LoadAssetAtPath<ScenePurposeConfiguration>(GetPreviousSelectedFilePath());
                        currentSelectedFile.IsPreviousSelected = false;

                        SetPreviousSelectedFilePath($" {DESTROYED_FILE_PREFIX} {GetCurrentSelectedFilePath()}");
                        SetCurrentSelectedFilePath(NONE_FILE_NAME);
                    }
                }
            }

            if (AllInstances.Count > 1)
            {
                string selectedFilePath = GetCurrentSelectedFilePath();
                string previousSelectedFilePath = GetPreviousSelectedFilePath();

                selectedFilePath += _userTarget.IsSelected ? $" {THIS_FILE_SUFIX}" : string.Empty;
                string text = $"{FILES_REPORT_HEADER}\n\n{FILES_REPORT_CURRENT_SELECTED_TAG} {selectedFilePath}";

                if (!string.IsNullOrEmpty(previousSelectedFilePath))
                {
                    previousSelectedFilePath += _userTarget.IsPreviousSelected ? $" {THIS_FILE_SUFIX}" : string.Empty;
                    text += $"\n\n{FILES_REPORT_PREVIOUS_SELECTED_TAG} {previousSelectedFilePath}";
                }

                if (AllInstances.Count > 2)
                {
                    text += $"\n\n{FILES_REPORT_IDLE_FILES_LIST_HEADER}";
                    for (int i = 0; i < AllInstances.Count; i++)
                    {
                        if (AllInstances[i] != GetCurrentSelectedFilePath())
                        {
                            text += $"\n{FILES_REPORT_LIST_DOT} {AllInstances[i]}";
                        }
                    }
                }

                EditorGUILayout.HelpBox(
                    text,
                    MessageType.None
                );
            }
        }

        private void UpdateScenePurposeConfiguration(ScenePurposeConfiguration scenePurposeContainer)
        {
            UIPModuleIntegrator.SetScenePurposeConfiguration(scenePurposeContainer);
        }

        private List<string> GetPreviousAllInstances()
        {
            if (_previousAllInstances == null || _previousAllInstances.Count == 0)
            {
                string concatenatedString = EditorPrefs.GetString(EDITORPREFS_PREVIOUS_ALL_INSTANCES_KEY, string.Empty);
                _previousAllInstances = new List<string>(concatenatedString.Split(PREVIOUS_ALL_INSTANCES_REMAP_SEPARATOR));
            }
            return _previousAllInstances;
        }

        private void SetPreviousAllInstances(List<string> value)
        {
            _previousAllInstances = new List<string>(value);
            string concatenatedString = string.Join(PREVIOUS_ALL_INSTANCES_REMAP_SEPARATOR, value);
            EditorPrefs.SetString(EDITORPREFS_PREVIOUS_ALL_INSTANCES_KEY, concatenatedString);
        }

        private string GetPreviousSelectedFilePath()
        {
            if (string.IsNullOrEmpty(_previousSelectedFilePath))
            {
                _previousSelectedFilePath = EditorPrefs.GetString(EDITORPREFS_PREVIOUS_SELECTED_FILE_KEY, NONE_FILE_NAME);
            }
            return _previousSelectedFilePath;
        }

        private void SetPreviousSelectedFilePath(string value)
        {
            if (!AllInstances.Contains(GetPreviousSelectedFilePath()))
            {
                AllInstances.Add(GetPreviousSelectedFilePath());
            }
            AllInstances[AllInstances.IndexOf(GetPreviousSelectedFilePath())] = value;
            EditorPrefs.SetString(EDITORPREFS_PREVIOUS_SELECTED_FILE_KEY, value);
            _previousSelectedFilePath = value;
        }

        private string GetCurrentSelectedFilePath()
        {
            if (string.IsNullOrEmpty(_currentSelectedFilePath))
            {
                _currentSelectedFilePath = EditorPrefs.GetString(EDITORPREFS_CURRENT_SELECTED_FILE_KEY, NONE_FILE_NAME);
            }
            return _currentSelectedFilePath;
        }

        private void SetCurrentSelectedFilePath(string value)
        {
            if (!AllInstances.Contains(GetCurrentSelectedFilePath()))
            {
                AllInstances.Add(GetCurrentSelectedFilePath());
            }
            AllInstances[AllInstances.IndexOf(GetCurrentSelectedFilePath())] = value;
            EditorPrefs.SetString(EDITORPREFS_CURRENT_SELECTED_FILE_KEY, value);
            _currentSelectedFilePath = value;
        }
    }
}