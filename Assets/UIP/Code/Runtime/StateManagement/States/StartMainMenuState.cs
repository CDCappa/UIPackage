using UIP.Runtime.UIManagement.Scenes;

namespace UIP.Runtime.StateManagement.States
{
    public class StartMainMenuState : UIStateBase<MainMenuSceneUIManager>
    {
        public StartMainMenuState(MainMenuSceneUIManager uiManager) : base(uiManager, isSubState: false, isTransitionalState: false) { }

        public override void EnterState()
        {
            uiManager.ShowMainMenu(Name);
        }

        public override void ExitState()
        {
            uiManager.HideMainMenu();
        }

        public override void OnEnterFirstSubState() { }

        public override void OnExitAllSubStates() { }

        public override void UpdateState() { }

        public override void LateUpdateState() { }
    }
}