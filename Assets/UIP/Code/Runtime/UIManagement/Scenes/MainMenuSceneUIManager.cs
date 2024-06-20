using TMPro;

using UIP.Runtime.Core.SceneManagement;

using UnityEngine;

namespace UIP.Runtime.UIManagement.Scenes
{
    public class MainMenuSceneUIManager : SceneUIManager
    {
        [SerializeField] private GameObject _mainMenu;
        [SerializeField] private TextMeshProUGUI _mainMenuHeader;
        [SerializeField] private GameObject _options;
        [SerializeField] private TextMeshProUGUI _optionsHeader;
        [SerializeField] private GameObject _darkCover;
        [SerializeField] private GameObject _exitConfirmationPopup;
        [SerializeField] private TextMeshProUGUI _exitConfirmationText;

        public void ShowMainMenu(string stateName)
        {
            _mainMenu.SetActive(true);
            SetStateNameInUiText(_mainMenuHeader, stateName);
        }

        public void HideMainMenu()
        {
            _mainMenu.SetActive(false);
        }

        public void ShowLoadingGameScreen(string stateName)
        {
            SceneManager.LoadNewUIScene(ScenePurpose.LOADING);
        }

        public void ShowOptionsScreen(string stateName)
        {
            _options.SetActive(true);
            SetStateNameInUiText(_optionsHeader, stateName);
        }

        public void HideOptionsScreen()
        {
            _options.SetActive(false);
        }

        public void ShowExitConfirmationPopup(string stateName)
        {
            _darkCover.SetActive(true);
            _exitConfirmationPopup.SetActive(true);
            SetStateNameInUiText(_exitConfirmationText, stateName);
        }

        public void HideExitConfirmationPopup()
        {
            _darkCover.SetActive(false);
            _exitConfirmationPopup.SetActive(false);
        }
    }
}