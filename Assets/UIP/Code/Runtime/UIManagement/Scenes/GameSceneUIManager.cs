using TMPro;

using UIP.Runtime.Core.SceneManagement;

using UnityEngine;

namespace UIP.Runtime.UIManagement.Scenes
{
    public class GameSceneUIManager : SceneUIManager
    {
        [SerializeField] private GameObject _openGameOptionsButton;
        [SerializeField] private GameObject _gameOptions;
        [SerializeField] private TextMeshProUGUI _gameOptionsHeader;
        [SerializeField] private GameObject _gameDarkCover;
        [SerializeField] private GameObject _gameExitConfirmationPopup;
        [SerializeField] private TextMeshProUGUI _gameExitConfirmationText;

        public void ShowGameScreen(string stateName)
        {
            _openGameOptionsButton.SetActive(true);
        }

        public void HideGameScreen()
        {
            _openGameOptionsButton.SetActive(false);
        }

        public void ShowOptionsScreen(string stateName)
        {
            _gameOptions.SetActive(true);
            SetStateNameInUiText(_gameOptionsHeader, stateName);
        }

        public void HideOptionsScreen()
        {
            _gameOptions.SetActive(false);
        }

        public void ShowExitConfirmationPopup(string stateName)
        {
            _gameDarkCover.SetActive(true);
            _gameExitConfirmationPopup.SetActive(true);
            SetStateNameInUiText(_gameExitConfirmationText, stateName);
        }

        public void HideExitConfirmationPopup()
        {
            _gameDarkCover.SetActive(false);
            _gameExitConfirmationPopup.SetActive(false);
        }

        public void ShowMainMenuScreen(string stateName)
        {
            SceneManager.LoadNewUIScene(ScenePurpose.MAIN_MENU);
        }
    }
}