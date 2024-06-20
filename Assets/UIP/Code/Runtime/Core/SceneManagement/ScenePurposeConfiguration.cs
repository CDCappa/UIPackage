using UnityEngine;

namespace UIP.Runtime.Core.SceneManagement
{
    [CreateAssetMenu(fileName = "ScenePurposeConfiguration", menuName = "UIP/Configuration/Scene Purpose Configuration")]
    public class ScenePurposeConfiguration : UIPScenePurposeInternal
    {
        [SerializeField] [HideInInspector] private bool _isValid;
        [SerializeField] [HideInInspector] private bool _isPreviousValid;

        public bool IsSelected {
            get { return _isValid; }
            set { _isValid = value; }
        }

        public bool IsPreviousSelected {
            get { return _isPreviousValid; }
            set { _isPreviousValid = value; }
        }
    }
}