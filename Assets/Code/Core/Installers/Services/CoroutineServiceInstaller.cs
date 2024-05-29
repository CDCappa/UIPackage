using UIPackage.Core.Installers;
using UIPackage.Core.Services;

using UnityEngine;

namespace UIPackage.Installers.Services
{
    public class CoroutineServiceInstaller : Installer
    {
        [SerializeField] private MonoBehaviourService _monoBehaviourService;

        public override void Install(ServiceLocator serviceLocator)
        {
            _monoBehaviourService.Setup();
            serviceLocator.RegisterService<ICoroutineService>(_monoBehaviourService as CoroutineService);
        }
    }
}