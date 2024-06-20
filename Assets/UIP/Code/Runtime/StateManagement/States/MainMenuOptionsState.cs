using UIP.Runtime.UIManagement.Scenes;

namespace UIP.Runtime.StateManagement.States
{
    public class MainMenuOptionsState : UIStateBase<MainMenuSceneUIManager>
    {
        public MainMenuOptionsState(MainMenuSceneUIManager uiManager) : base(uiManager, isSubState: false, isTransitionalState: false) { }

        public override void EnterState()
        {
            uiManager.ShowOptionsScreen(Name);
        }

        public override void ExitState()
        {
            uiManager.HideOptionsScreen();
        }

        public override void OnEnterFirstSubState() { }

        public override void OnExitAllSubStates() { }

        public override void UpdateState() { }

        public override void LateUpdateState() { }
    }
}