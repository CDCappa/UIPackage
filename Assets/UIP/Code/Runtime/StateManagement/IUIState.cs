namespace UIP.Runtime.StateManagement
{
    public interface IUIState
    {
        public string Name { get; }
        public bool IsSubState { get; }
        public bool IsTransitionalState { get; }
        public void Initialize();
        public void EnterState();
        public void ExitState();
        public void OnEnterFirstSubState();
        public void OnExitAllSubStates();
        public void UpdateState();
        public void LateUpdateState();
    }
}