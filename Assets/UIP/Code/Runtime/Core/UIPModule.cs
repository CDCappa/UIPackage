using UIP.Runtime.Services;
using UIP.Runtime.StateManagement;
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

        private IUIState[] _uiStates;
        private SceneUIManager _uiManager;
        private UIStateManager _uiStateManager;
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

        /// <summary>
        /// TODO: Provisional Method: OnSceneLoaded temporarily simulates the future linkage between actions, states, and inputs that will be implemented once the input management system is fully integrated.
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
        {
            string sceneName = scene.name;

            Debug.Log($"<color=#ffff00>[UIP]</color>{mode.ToString().ToUpper()} Scene loaded: {sceneName}.");

            _uiManager = FindAnyObjectByType<SceneUIManager>();

            if (_uiManager == null)
            {
                return;
            }
            else
            {
                _uiStateManager = new UIStateManager();

                if (SceneManager.CurrentPurpose.Equals(ScenePurpose.MAIN_MENU))
                {
                    _uiStates = _uiStateManager.CreateStates(_uiManager);
                }
                else if (SceneManager.CurrentPurpose.Equals(ScenePurpose.LOADING))
                {
                    _uiStates = _uiStateManager.CreateStates(_uiManager);
                }
                else if (SceneManager.CurrentPurpose.Equals(ScenePurpose.GAME))
                {
                    _uiStates = _uiStateManager.CreateStates(_uiManager);
                }
                if (_uiStates != null)
                {
                    _uiStateManager.ChangeState(_uiStates[0]);
                }
            }
        }

        /// <summary>
        /// TODO: Provisional Method: this Update temporarily simulates the future implementation of the input management system.
        /// </summary>
        private void Update()
        {
            if (SceneManager.CurrentPurpose.Equals(ScenePurpose.STARTUP))
            {
                return;
            }

            if (_uiStateManager != null && _uiStateManager.CurrentState != null && _uiStateManager.CurrentState.IsTransitionalState)
                return;

            if (Input.GetKeyUp(KeyCode.Escape))
            {
                Debug.Log($"<color=#FFA500>[UIP]</color> KeyCode.Escape.");
                _uiStateManager.ExitLastSubState();
            }

            if (SceneManager.CurrentPurpose.Equals(ScenePurpose.MAIN_MENU))
            {
                if (_uiStateManager.CurrentStateOrSubstate == _uiStates[0])
                {
                    if (Input.GetKeyUp(KeyCode.O))
                    {
                        Debug.Log($"<color=#FFA500>[UIP]</color> KeyCode.O.");
                        _uiStateManager.ChangeState(_uiStates[1]);
                    }

                    else if (Input.GetKeyUp(KeyCode.E))
                    {
                        Debug.Log($"<color=#FFA500>[UIP]</color> KeyCode.E.");
                        _uiStateManager.ChangeState(_uiStates[2]);
                    }

                    else if (Input.GetKeyUp(KeyCode.G))
                    {
                        Debug.Log($"<color=#FFA500>[UIP]</color> KeyCode.G.");
                        _uiStateManager.ChangeState(_uiStates[3]);
                    }
                }
                else if (_uiStateManager.CurrentStateOrSubstate == _uiStates[1])
                {
                    if (Input.GetKeyUp(KeyCode.M))
                    {
                        Debug.Log($"<color=#FFA500>[UIP]</color> KeyCode.M.");
                        _uiStateManager.ChangeState(_uiStates[0]);
                    }
                }
            }
            else if (SceneManager.CurrentPurpose.Equals(ScenePurpose.GAME))
            {
                if (_uiStateManager.CurrentStateOrSubstate == _uiStates[0])
                {
                    if (Input.GetKeyUp(KeyCode.O))
                    {
                        Debug.Log($"<color=#FFA500>[UIP]</color> KeyCode.O.");
                        _uiStateManager.ChangeState(_uiStates[1]);
                    }
                }
                else if (_uiStateManager.CurrentStateOrSubstate == _uiStates[1])
                {
                    if (Input.GetKeyUp(KeyCode.E))
                    {
                        Debug.Log($"<color=#FFA500>[UIP]</color> KeyCode.E.");
                        _uiStateManager.ChangeState(_uiStates[2]);
                    }
                }
                else if (_uiStateManager.CurrentStateOrSubstate == _uiStates[2])
                {
                    if (Input.GetKeyUp(KeyCode.M))
                    {
                        Debug.Log($"<color=#FFA500>[UIP]</color> KeyCode.M.");
                        _uiStateManager.ChangeState(_uiStates[3]);
                    }
                }
            }
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