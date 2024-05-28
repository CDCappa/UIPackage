using UnityEngine;

namespace UIPackage.Core.Installers
{
    public class MonoBehaviourService : MonoBehaviour
    {
        public void Setup()
        {
            transform.SetParent(null, false);
            DontDestroyOnLoad(gameObject);
        }
    }
}