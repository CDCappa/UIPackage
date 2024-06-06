using UIP.Runtime.Services;

using UnityEngine;

namespace UIP.Runtime.Core.Installers
{
	public abstract class Installer : MonoBehaviour
	{
		public abstract void Install(ServiceLocator serviceLocator);
	}
}
