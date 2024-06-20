using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

using VanillaScene = UnityEngine.SceneManagement.Scene;
using VanillaSceneManager = UnityEngine.SceneManagement.SceneManager;

namespace UIP.Runtime.Core.SceneManagement
{
    public static class SceneManager
    {
        public const string ASSETS_SCENES_LOCATION = "/UIP/Scenes/";

        public static event Action<VanillaScene, LoadSceneMode> OnSceneLoaded;
        public static UIPScenePurposeInternal UIPScenePurposeInternal { private set; get; }
        public static ScenePurposeConfiguration ScenePurposeConfiguration { private set; get; }
        public static Dictionary<string, string> GameSceneNameByUIPSceneName { get; private set; }
        public static ScenePurpose CurrentPurpose { get; private set; }

        public static void Initialize(
            UIPScenePurposeInternal uipScenePurposeInternal,
            ScenePurposeConfiguration scenePurposeConfiguration)
        {
            UIPScenePurposeInternal = uipScenePurposeInternal;
            ScenePurposeConfiguration = scenePurposeConfiguration;

            VanillaSceneManager.sceneLoaded += HandleSceneLoaded;

            SetupScenesLinks();
        }

        private static void SetupScenesLinks()
        {
            Array enumValues = Enum.GetValues(typeof(ScenePurpose));
            ScenePurpose[] purposes = (ScenePurpose[])enumValues;

            GameSceneNameByUIPSceneName = new();

            for (int i = 1; i < purposes.Length; i++)
            {
                UnityEditor.SceneAsset uipScene = UIPScenePurposeInternal.GetScene(purposes[i]).Asset;
                UnityEditor.SceneAsset gameScene = ScenePurposeConfiguration.GetScene(purposes[i]).Asset;

                if (gameScene == null)
                {
                    ScenePurposeConfiguration.SetSceneAsset(purposes[i], UIPScenePurposeInternal.GetScene(ScenePurpose.NONE).Asset);
                }

                gameScene = ScenePurposeConfiguration.GetScene(purposes[i]).Asset;

                GameSceneNameByUIPSceneName[uipScene.name] = gameScene.name;

                Debug.Log("<color=#ffff00>[UIP]</color> " + purposes[i] + " | " + uipScene.name + " | " + gameScene.name);
            }
        }

        public static void LoadNewUIScene(ScenePurpose purpose, bool forceUIPOnly = false) => LoadNewUIScene(UIPScenePurposeInternal.GetScene(purpose).Asset.name, forceUIPOnly);

        public static void LoadNewUIScene(string sceneName, bool forceUIPOnly = false)
        {
            if (sceneName.Equals(UIPScenePurposeInternal.GetScene(ScenePurpose.NONE).Asset.name))
            {
                return;
            }

            for (int i = 1; i < VanillaSceneManager.sceneCount; i++)
            {
                VanillaScene scene = VanillaSceneManager.GetSceneAt(i);

                if (scene.isLoaded)
                {
                    if (scene.name.Equals(sceneName))
                    {
                        Debug.LogError($"[UIP] An attempt to load an additive scene that is already currently loaded was interrupted ({sceneName}).");
                        return;
                    }

                    if (IsUIPackageInteractiveScene(scene))
                    {
                        VanillaSceneManager.UnloadSceneAsync(scene);
                    }
                }
            }

            CurrentPurpose = UIPScenePurposeInternal.GetScene(sceneName).Purpose;

            bool isBootstrapScene = sceneName.Equals(UIPScenePurposeInternal.GetScene(ScenePurpose.STARTUP).Asset.name);
            if (!forceUIPOnly && !isBootstrapScene)
            {
                VanillaSceneManager.LoadScene(GameSceneNameByUIPSceneName[sceneName], LoadSceneMode.Single);
            }
            VanillaSceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }

        private static bool IsUIPackageInteractiveScene(VanillaScene scene) =>
            scene.path.Replace("Assets", Application.dataPath) == $"{Application.dataPath}{ASSETS_SCENES_LOCATION}{scene.name}.unity";

        private static void HandleSceneLoaded(VanillaScene scene, LoadSceneMode mode) => OnSceneLoaded?.Invoke(scene, mode);

        public static void Shutdown() => VanillaSceneManager.sceneLoaded -= HandleSceneLoaded;
    }
}