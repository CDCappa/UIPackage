using UIP.Runtime.UIManagement.Scenes;

namespace UIP.Runtime.StateManagement.States
{
    public class MainMenuTransitionToLoadingGameState : UIStateBase<MainMenuSceneUIManager>
    {
        public MainMenuTransitionToLoadingGameState(MainMenuSceneUIManager uiManager) : base(uiManager, isSubState: false, isTransitionalState: true) { }

        public override void EnterState()
        {
            uiManager.ShowLoadingGameScreen(Name);
        }

        public override void ExitState() { }

        public override void OnEnterFirstSubState() { }

        public override void OnExitAllSubStates() { }

        public override void UpdateState() { }

        public override void LateUpdateState() { }
    }
}