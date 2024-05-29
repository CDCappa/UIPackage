using UIPackage.Core.Services;

using UnityEngine;

namespace UIPackage.Core.Installers
{
	public abstract class Installer : MonoBehaviour
	{
		public abstract void Install(ServiceLocator serviceLocator);
	}
}
