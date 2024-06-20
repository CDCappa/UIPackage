using UIP.Runtime.UIManagement.Scenes;

namespace UIP.Runtime.StateManagement.States
{
    public class GameExitConfirmationPopupState : UIStateBase<GameSceneUIManager>
    {
        public GameExitConfirmationPopupState(GameSceneUIManager uiManager) : base(uiManager, isSubState: true, isTransitionalState: false) { }

        public override void EnterState()
        {
            uiManager.ShowExitConfirmationPopup(Name);
        }

        public override void ExitState()
        {
            uiManager.HideExitConfirmationPopup();
        }

        public override void OnEnterFirstSubState() { }

        public override void OnExitAllSubStates() { }

        public override void UpdateState() { }

        public override void LateUpdateState() { }
    }
}