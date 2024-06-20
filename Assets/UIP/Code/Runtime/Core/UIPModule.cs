using UIP.Runtime.Services;
using UIP.Runtime.UIManagement;
using UIP.Runtime.Core.SceneManagement;
using UnityEngine;

namespace UIP.Runtime.Core
{
    public class UIPModule : MonoBehaviour
    {
        public static UIPModule Instance => _instance; // TODO: It remains to be determined whether UIPModule will need to have public access members.

        #region private
        private static UIPModule _instance;

        private SceneUIManager _uiManager;
        private ICoroutineService _coroutineService;
        private ICoroutineService CoroutineService => _coroutineService ??= GetService<ICoroutineService>();

        private void CreateBootstrapScene() => SceneManager.LoadNewUIScene(ScenePurpose.STARTUP);

        private static void SetupInstance(GameObject prefab)
        {
            GameObject moduleGameObject = Instantiate(prefab);
            moduleGameObject.name = typeof(UIPModule).Name;
            _instance = moduleGameObject.GetComponent<UIPModule>();
            DontDestroyOnLoad(_instance);
        }

        private void SubscribeToEvents()
        {
            SceneManager.OnSceneLoaded += Instance.OnSceneLoaded;
        }

        private void UnsubscribeFromEvents()
        {
            SceneManager.OnSceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            CoroutineService.WaitForSeconds(this, () => SceneManager.LoadNewUIScene(ScenePurpose.MAIN_MENU), 2, true);
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
            StopSceneManager();
        }

        private void StopSceneManager() => SceneManager.Shutdown();

        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            string sceneName = scene.name;

            Debug.Log($"<color=#ffff00>[UIP]</color>{mode.ToString().ToUpper()} Scene loaded: {sceneName}.");
        }

        private T GetService<T>() => ServiceLocator.Instance.GetService<T>();
        #endregion

        public static void Initialize(GameObject prefab) // TODO: It remains to be determined whether UIPModule will need to have public access members.
        {
            SetupInstance(prefab);
            Instance.SubscribeToEvents();
            Instance.CreateBootstrapScene();
        }

        public void LoadBuiltInScene(string sceneName) => SceneManager.LoadNewUIScene(sceneName); // TODO: It remains to be determined whether UIPModule will need to have public access members.

        public void LoadBuiltInScene(ScenePurpose scenePurpose) => SceneManager.LoadNewUIScene(scenePurpose); // TODO: It remains to be determined whether UIPModule will need to have public access members.
    }
}