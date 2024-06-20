using System;

using TMPro;

using UIP.Runtime.Common.Extensions;
using UIP.Runtime.Services;

using UnityEngine;

namespace UIP.Runtime.UIManagement
{
    public abstract class SceneUIManager : MonoBehaviour
    {
        protected ICoroutineService coroutineService;
        protected ICoroutineService CoroutineService => coroutineService ??= GetService<ICoroutineService>();

        private Action _update;
        private Action _lateUpdate;


        protected virtual void Update() => _update?.Invoke();

        private void LateUpdate() => _lateUpdate?.Invoke();

        public void SetUpdate(Action update) => _update = update;

        public void SetLateUpdate(Action lateUpdate) => _lateUpdate = lateUpdate;

        #region Utils
        protected void SetStateNameInUiText(TextMeshProUGUI uiRef, string stateName)
        {
            string formatedStateName = StateNameToUiFormat(stateName);
            uiRef.text = formatedStateName;
        }

        protected string StateNameToUiFormat(string stateName) => stateName.Replace("State", "").ToSpacedUpperCase();

        protected T GetService<T>() => ServiceLocator.Instance.GetService<T>();
        #endregion
    }
}