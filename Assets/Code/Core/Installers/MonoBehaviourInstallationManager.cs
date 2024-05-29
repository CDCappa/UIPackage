using System.Collections.Generic;

using UIPackage.Core.Services;

using UnityEngine;

namespace UIPackage.Core.Installers
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
