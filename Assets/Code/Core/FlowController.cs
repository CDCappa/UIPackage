using Core;

using UIPackage.Services.Coroutines;

using UnityEngine;

public class FlowController : MonoBehaviour
{
    private ICoroutineService _coroutineService;
    private ICoroutineService CoroutineService => _coroutineService ??= GetService<ICoroutineService>();

    void Start()
    {
        for (int i = 0; i < 5; i++) CoroutineService.WaitForSeconds(this, ShowSuccesLog, i);
    }

    private T GetService<T>() => ServiceLocator.Instance.GetService<T>();

    private void ShowSuccesLog() => Debug.Log("SUCCES!");
}