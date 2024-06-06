using UnityEngine;

namespace UIP.Runtime.Services
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