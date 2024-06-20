using TMPro;

using UIP.Runtime.Core.SceneManagement;

using UnityEngine;

namespace UIP.Runtime.UIManagement.Scenes
{
    public class LoadingGameSceneUIManager : SceneUIManager
    {
        [SerializeField] private GameObject _darkCover;
        [SerializeField] private GameObject _loading;
        [SerializeField] private TextMeshProUGUI _loadingText;

        public void ShowLoadingScreen(string stateName)
        {
            _darkCover.SetActive(true);
            _loading.SetActive(true);

            CoroutineService.PerformingActionsOverTime(
                owner: null,
                actionByTime: TransitionToGameState,
                actionInterval: .5f,
                callback: () => SceneManager.LoadNewUIScene(ScenePurpose.GAME),
                totalTime: 5, // Simulate a loading time.
                useUnscaledTime: true
                );
        }

        private void TransitionToGameState(float seconds, float totalSeconds)
        {
            float percent = seconds / totalSeconds;
            int dotCount = Mathf.FloorToInt(Mathf.Repeat(percent * 10, 4));
            _loadingText.text = "Loading" + new string('.', dotCount);
        }
    }
}