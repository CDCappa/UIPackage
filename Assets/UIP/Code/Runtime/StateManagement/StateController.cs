using UnityEngine;

namespace UIP.Runtime.StateManagement
{
    public class StateController : MonoBehaviour // DEPRECATED
    {
        private static StateController _instance;

        private const string MESSAGE_COLOR_LOG = "#ffff00";
        private const string MESSAGE_COLOR_WARNING_LOG = "#ff8000";

        public static StateController Instance {
            get {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<StateController>();
                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(StateController).Name);
                        _instance = singletonObject.AddComponent<StateController>();
                    }
                }
                return _instance;
            }
        }

        public IUIState CurrentState { get; private set; }
        public IUIState PreviousState { get; private set; }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ChangeUIState(IUIState newState)
        {
            if (!Equals(newState, CurrentState))
            {
                Debug.Log($"[GUI-StateController] Change UI State from to:\n<color={MESSAGE_COLOR_LOG}>{CurrentState} → {newState}</color>");
                PreviousState = CurrentState;
                CurrentState = newState;
            }
            else
            {
                Debug.LogWarning($"[GUI-StateController] There is a double call to change state to <color={MESSAGE_COLOR_WARNING_LOG}>{newState}</color>");
            }
        }
    }
}