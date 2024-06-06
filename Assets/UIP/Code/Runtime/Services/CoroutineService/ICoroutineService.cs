using System;
using System.Collections;

namespace UIP.Runtime.Services
{
    public interface ICoroutineService
    {
        bool IsCoroutineActive(object owner, string name);
        Guid StartCoroutine(object owner, IEnumerator coroutine, float timeout = 0, bool useUnscaledTime = false);
        void StartUniqueCoroutine(object owner, IEnumerator coroutine, string name, float timeout = 0, bool useUnscaledTime = false);
        void StopAllCoroutinesFromOneOwner(object owner);
        void StopCoroutine(object owner, Guid coroutineId);
        void StopUniqueCoroutine(object owner, string name);
        void WaitForSeconds(object owner, Action action, float totalTime, bool useUnscaledTime = false);
        void PerformingActionsOverTime(object owner, Action<float, float> actionOverTime, Action callback, float totalTime, bool useUnscaledTime = false);
        void PerformingActionsOverTime(object owner, Action<float, float> actionByTime, float actionInterval, Action callback, float totalTime, bool useUnscaledTime = false);
        void WaitForCountOfFrames(object owner, Action action, int frameDelayCount = 1);
        void WaitForEndOfCurrentFrame(Action action);
        void WaitUntilCondition(object owner, Func<bool> condition, Action action);
        void WaitUntilCondition(object owner, Func<bool> condition, Action<float> actionOverTime, Action callback, bool useUnscaledTime = false);
        void WaitUntilCondition(object owner, Func<bool> condition, Action<float> actionOverTime, float actionInterval, Action callback, bool useUnscaledTime = false);
        void Dispose();
    }
}