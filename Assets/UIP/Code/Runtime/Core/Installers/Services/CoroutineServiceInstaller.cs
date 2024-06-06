using UIP.Runtime.Services;

using UnityEngine;

namespace UIP.Runtime.Core.Installers.Services
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