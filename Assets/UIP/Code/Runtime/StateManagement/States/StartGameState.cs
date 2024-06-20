using UIP.Runtime.UIManagement.Scenes;

namespace UIP.Runtime.StateManagement.States
{
    public class StartGameState : UIStateBase<GameSceneUIManager>
    {
        public StartGameState(GameSceneUIManager uiManager) : base(uiManager, isSubState: false, isTransitionalState: false) { }

        public override void EnterState()
        {
            uiManager.ShowGameScreen(Name);
        }

        public override void ExitState()
        {
            uiManager.HideGameScreen();
        }

        public override void OnEnterFirstSubState() { }

        public override void OnExitAllSubStates() { }

        public override void UpdateState() { }

        public override void LateUpdateState() { }
    }
}