using System.Collections.Generic;

using UIP.Runtime.Services;

using UnityEngine;

namespace UIP.Runtime.Core.Installers
{
    public class MonoBehaviourInstallationManager : MonoBehaviour
    {
        [SerializeField] private List<Installer> _installers;

        private void Awake()
        {
            InstallDependencies();
        }

        private void InstallDependencies()
        {
            foreach (Installer installer in _installers)
            {
                installer.Install(ServiceLocator.Instance);
            }
        }
    }
}
