using System;
using System.Linq;

using UIP.Runtime.Core.SceneManagement;

using UnityEditor;

using UnityEngine;

namespace UIP.Runtime.Core.Initialization
{
    [Serializable]
    public static class UIPModuleIntegrator
    {
        private static GameObject UIPModulePrefab;
        private static UIPScenePurposeInternal UIPScenePurposeInternal;
        private static ScenePurposeConfiguration ScenePurposeConfiguration;

        public static void SetUIPModulePrefab(GameObject prefab) => UIPModulePrefab = prefab;
        public static void SetUIPScenePurposeInternal(UIPScenePurposeInternal config) => UIPScenePurposeInternal = config;
        public static void SetScenePurposeConfiguration(ScenePurposeConfiguration config) => ScenePurposeConfiguration = config;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Integrate() => EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                InitializeUIP();
            }
        }

        private static void InitializeUIP()
        {
            Debug.Log("<color=#00ffff>[UIP]</color> Running...");

            ScenePurposeConfiguration = Resources.Load<ScenePurposeConfiguration>("ScenePurposeConfiguration"); // TODO: Using strings this way as a parameter to find objects is a bad practice, and remains as technical debt.
            //ScenePurposeConfiguration = FindScriptableObjectResource<ScenePurposeConfiguration>(); // TODO: Review this method, it is not finding the scriptable objects.
            UIPScenePurposeInternal = Resources.Load<UIPScenePurposeInternal>("UIPScenePurposeInternal"); // TODO: Using strings this way as a parameter to find objects is a bad practice, and remains as technical debt.
            //UIPScenePurposeInternal = FindScriptableObjectResource<UIPScenePurposeInternal>(); // TODO: Review this method, it is not finding the scriptable objects.

            UIPModulePrefab = FindPrefabResource<UIPModule>().gameObject;

            SceneManager.Initialize(UIPScenePurposeInternal, ScenePurposeConfiguration);

            UIPModule.Initialize(UIPModulePrefab);

            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private static T FindPrefabResource<T>() where T : Component
        {
            GameObject[] allResources = Resources.LoadAll<GameObject>("");
            return allResources
                .Select(prefab => prefab.GetComponent<T>())
                .FirstOrDefault(component => component != null);
        }

        private static T FindScriptableObjectResource<T>() where T : UnityEngine.Object // TODO: Review this method, it is not finding the scriptable objects.
        {
            UnityEngine.Object[] allResources = Resources.LoadAll<UnityEngine.Object>("");
            T result = allResources.OfType<T>().FirstOrDefault();
            return result;
        }
    }
}