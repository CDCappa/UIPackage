using UnityEngine;

namespace UIP.Runtime.Services
{
    public class MonoBehaviourService : MonoBehaviour
    {
        [SerializeField] private bool _separateInstanceInHierarchy = false;

        public void Setup()
        {
            if (_separateInstanceInHierarchy)
            {
                transform.SetParent(null, false);
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}