using UIP.Runtime.UIManagement;

namespace UIP.Runtime.StateManagement.States
{
    public abstract class UIStateBase<T> : IUIState where T : SceneUIManager
    {
        protected T uiManager;

        private readonly string _name;
        private readonly bool _isSubState;
        private readonly bool _isTransitionalState;

        public string Name => _name;
        public bool IsSubState => _isSubState;
        public bool IsTransitionalState => _isTransitionalState;

        public UIStateBase(T uiManager, bool isSubState, bool isTransitionalState)
        {
            _name = this.GetType().Name;
            _isSubState = isSubState;
            _isTransitionalState = isTransitionalState;
            this.uiManager = uiManager;
        }

        public void Initialize()
        {
            uiManager.SetUpdate(UpdateState);
            uiManager.SetLateUpdate(LateUpdateState);
        }

        public abstract void EnterState();
        public abstract void ExitState();
        public abstract void OnEnterFirstSubState();
        public abstract void OnExitAllSubStates();
        public abstract void UpdateState();
        public abstract void LateUpdateState();
    }
}