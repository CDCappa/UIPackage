using UIP.Runtime.UIManagement;

namespace UIP.Runtime.StateManagement
{
    public interface IUIStateManager
    {
        public IUIState CurrentStateOrSubstate { get; }
        public IUIState CurrentState { get; }
        public IUIState PreviousState { get; }
        public System.Collections.Generic.List<IUIState> ActiveSubStates { get; }
        public IUIState LastSubState { get; }
        public IUIState PreviousSubState { get; }

        public void ChangeState(IUIState newState);
        public IUIState[] CreateStates(SceneUIManager uiManager);
        public void ExitSubState(IUIState subState);
        public void ExitLastSubState();
        public void ExitAllSubStates();
    }
}