using Core;

using UIPackage.Core.Installers;
using UIPackage.Services.Coroutines;

using UnityEngine;

namespace UIPackage.Services.Installers
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