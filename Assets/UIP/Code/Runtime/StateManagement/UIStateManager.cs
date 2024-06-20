using System.Collections.Generic;
using System.Linq;

using UIP.Runtime.StateManagement.States;
using UIP.Runtime.UIManagement;
using UIP.Runtime.UIManagement.Scenes;

namespace UIP.Runtime.StateManagement
{
    public class UIStateManager : IUIStateManager
    {
        public IUIState CurrentStateOrSubstate => LastSubState ?? _currentState;
        public IUIState CurrentState => _currentState;
        public IUIState PreviousState => _previousState;
        public List<IUIState> ActiveSubStates => _activeSubStates;
        public IUIState LastSubState => _activeSubStates.LastOrDefault();
        public IUIState PreviousSubState => _activeSubStates.Count >= 2 ? _activeSubStates[_activeSubStates.Count - 2] : default(IUIState);

        private IUIState _currentState;
        private IUIState _previousState;
        private List<IUIState> _activeSubStates = new();

        public IUIState[] CreateStates(SceneUIManager uiManager)
        {
            if (uiManager is MainMenuSceneUIManager mainMenuSceneUIManager)
            {
                return CreateStates(mainMenuSceneUIManager);
            }
            else if (uiManager is GameSceneUIManager gameSceneUIManager)
            {
                return CreateStates(gameSceneUIManager);
            }
            else if (uiManager is LoadingGameSceneUIManager loadingGameSceneUIManager)
            {
                return CreateStates(loadingGameSceneUIManager);
            }
            else
            {
                return null;
            }
        }

        private IUIState[] CreateStates(MainMenuSceneUIManager uiManager)
        {
            return new IUIState[]
            {
                new StartMainMenuState(uiManager),
                new MainMenuOptionsState(uiManager),
                new MainMenuExitConfirmationPopupState(uiManager),
                new MainMenuTransitionToLoadingGameState(uiManager),
            };
        }

        private IUIState[] CreateStates(LoadingGameSceneUIManager uiManager)
        {
            return new IUIState[]
            {
                new LoadingGameTransitionToGameState(uiManager),
            };
        }

        private IUIState[] CreateStates(GameSceneUIManager uiManager)
        {
            return new IUIState[]
            {
                new StartGameState(uiManager),
                new GameOptionsState(uiManager),
                new GameExitConfirmationPopupState(uiManager),
                new GameTransitionToMainMenuState(uiManager),
            };
        }

        public void ChangeState(IUIState newState)
        {
            if (_currentState == newState)
            {
                UnityEngine.Debug.LogError($"<color=#ff0000>[UIP] There is a double state change call: {newState.Name}</color>");
            }
            else if (LastSubState == newState)
            {
                UnityEngine.Debug.LogError($"<color=#ff0000>[UIP] There is a double subState change call: {newState.Name}</color>");
            }
            else
            {
                if (newState.IsSubState)
                {
                    if (_currentState == null)
                    {
                        UnityEngine.Debug.LogError($"<color=#ff0000>[UIP] Substates cannot be created if there is no active state.</color>");
                        return;
                    }

                    _activeSubStates.Add(newState);
                    InitializeState(LastSubState);

                    PreviousSubState?.OnEnterFirstSubState();
                    if (_activeSubStates.Count == 1)
                    {
                        _currentState.OnEnterFirstSubState();
                    }
                }
                else
                {
                    if (_currentState != null)
                    {
                        ExitAllSubStates(warningLoggingIsAllowed: false);
                        _currentState.ExitState();
                    }
                    _previousState = _currentState;
                    _currentState = newState;
                    InitializeState(_currentState);
                }
            }
        }

        public void ExitSubState(IUIState subState)
        {
            if (subState.IsSubState)
            {
                if (_activeSubStates.Count > 0)
                {
                    if (_activeSubStates.Contains(subState))
                    {
                        subState.ExitState();
                        if (subState == LastSubState)
                        {
                            PreviousSubState?.OnExitAllSubStates();
                        }
                        _activeSubStates.Remove(subState);
                        if (_activeSubStates.Count == 0)
                        {
                            _currentState?.OnExitAllSubStates();
                        }
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning($"<color=#ffff00>[UIP] The substate you are trying to exit from ({subState.Name}) is not currently active.</color>");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning("<color=#ffff00>[UIP] There are no active substates at the moment.</color>");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning($"<color=#ffff00>[UIP] {subState.Name} is not configured as a substate.</color>");
            }
        }

        public void ExitLastSubState()
        {
            if (_activeSubStates.Count > 0)
            {
                LastSubState?.ExitState();
                _activeSubStates.Remove(LastSubState);
                LastSubState?.OnExitAllSubStates();
                _currentState?.OnExitAllSubStates();
            }
            else
            {
                UnityEngine.Debug.LogWarning("<color=#ffff00>[UIP] There are no active substates at the moment.</color>");
            }
        }

        public void ExitAllSubStates()
        {
            ExitAllSubStates(warningLoggingIsAllowed: true);
        }

        private void ExitAllSubStates(bool warningLoggingIsAllowed)
        {
            if (_activeSubStates.Count == 0)
            {
                if (warningLoggingIsAllowed)
                {
                    UnityEngine.Debug.LogWarning("<color=#ffff00>[UIP] There are no active substates at the moment.</color>");
                }
            }
            else
            {
                foreach (var subState in _activeSubStates)
                {
                    subState.ExitState();
                }
                _activeSubStates.Clear();
                _currentState?.OnExitAllSubStates();
            }
        }

        private void InitializeState(IUIState state)
        {
            state.Initialize();
            state.EnterState();
            UpdateStateLog(state);
        }

        private void UpdateStateLog(IUIState state)
        {
            string log = "<color=#ff00ff>[UIP]</color> ";

            string newStateText = GetStateText(state, false);
            string currentStateText = GetStateText(_currentState, true);
            string previousStateText = GetStateText(_previousState, true);

            if (state.IsSubState)
            {
                log += $"Add {_activeSubStates.IndexOf(state)+1}° SUB STATE: {newStateText} in {currentStateText}";
            }
            else
            {
                if (_previousState == null)
                {
                    log += $"Initiate the FIRST STATE of the current scene: {newStateText}";
                }
                else
                {
                    log += $"Change STATE: from {previousStateText} to STATE {newStateText}";
                }
            }

            if (state.IsSubState)
            {
                log += $"\nOrdered list of current active substates:";
                for (int i = 0; i < _activeSubStates.Count; i++)
                {
                    log += $"\n• [{i+1}]{_activeSubStates[i].Name}{(_activeSubStates[i].IsTransitionalState ? " (TRANSITIONAL)" : "")}";
                }
            }

            UnityEngine.Debug.Log(log);
        }

        private string GetStateText(IUIState state, bool addStateCategory)
        {
            if (state == null)
            {
                return string.Empty;
            }

            string stateText = state.IsTransitionalState ? "TRANSITIONAL " : "";
            if (addStateCategory)
            {
                stateText += state.IsSubState ? "SUB STATE " : "STATE ";
            }
            stateText += state.Name;

            return stateText;
        }
    }
}