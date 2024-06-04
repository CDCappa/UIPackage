using System;
using System.Collections;
using System.Collections.Generic;

using UIPackage.Core.Installers;

using UnityEngine;

namespace UIPackage.Core.Services
{
    public class CoroutineService : MonoBehaviourService, ICoroutineService, IDisposable
    {
        private Dictionary<object, Dictionary<string, Coroutine>> _activeUniqueCoroutines = new Dictionary<object, Dictionary<string, Coroutine>>();
        private Dictionary<object, Dictionary<Guid, Coroutine>> _activeCoroutines = new Dictionary<object, Dictionary<Guid, Coroutine>>();

        public void WaitForSeconds(object owner, Action action, float totalTime, bool useUnscaledTime = false)
        {
            StartCoroutine(owner ?? this, CO_WaitForSecondsCoroutine(action, totalTime, useUnscaledTime));
        }

        public void PerformingActionsOverTime(object owner, Action<float, float> actionPerFrame, Action callback, float totalTime, bool useUnscaledTime = false)
        {
            StartCoroutine(owner ?? this, CO_PerformingActionsOverTime(actionPerFrame, callback, totalTime, useUnscaledTime));
        }
        public void PerformingActionsOverTime(object owner, Action<float, float> actionByTime, float actionInterval, Action callback, float totalTime, bool useUnscaledTime = false)
        {
            StartCoroutine(owner ?? this, CO_PerformingActionsOverTime(actionByTime, actionInterval, callback, totalTime, useUnscaledTime));
        }

        public void WaitUntilCondition(object owner, Func<bool> condition, Action action)
        {
            StartCoroutine(owner ?? this, CO_WaitUntilConditionCoroutine(condition, action));
        }
        public void WaitUntilCondition(object owner, Func<bool> condition, Action<float> actionOverTime, Action callback, bool useUnscaledTime = false)
        {
            StartCoroutine(owner ?? this, CO_WaitUntilConditionCoroutine(condition, actionOverTime, callback, useUnscaledTime));
        }
        public void WaitUntilCondition(object owner, Func<bool> condition, Action<float> actionOverTime, float actionInterval, Action callback, bool useUnscaledTime = false)
        {
            StartCoroutine(owner ?? this, CO_WaitUntilConditionCoroutine(condition, actionOverTime, actionInterval, callback, useUnscaledTime));
        }

        public void WaitForEndOfCurrentFrame(Action action)
        {
            StartCoroutine(CO_WaitForEndOfFrameCoroutine(action));
        }

        public void WaitForCountOfFrames(object owner, Action action, int frameDelayCount = 1)
        {
            if (frameDelayCount > 1)
            {
                StartCoroutine(owner ?? this, CO_WaitForFramesCoroutine(action, frameDelayCount));
            }
            else
            {
                StartCoroutine(CO_WaitForFramesCoroutine(action, frameDelayCount));
            }
        }

        private IEnumerator CO_WaitForSecondsCoroutine(Action action, float totalTime, bool useUnscaledTime)
        {
            float elapsedTime = 0f;

            while (elapsedTime < totalTime)
            {
                elapsedTime += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                yield return null;
            }

            action.Invoke();
        }

        private IEnumerator CO_PerformingActionsOverTime(Action<float, float> actionOverTime, Action callback, float totalTime, bool useUnscaledTime)
        {
            float elapsedTime = 0f;

            while (elapsedTime < totalTime)
            {
                elapsedTime += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                actionOverTime.Invoke(elapsedTime, totalTime);
                yield return null;
            }

            callback?.Invoke();
        }
        private IEnumerator CO_PerformingActionsOverTime(Action<float, float> actionByTime, float actionInterval, Action callback, float totalTime, bool useUnscaledTime)
        {
            float elapsedTime = 0f;
            float nextActionTime = 0f;

            while (elapsedTime < totalTime)
            {
                elapsedTime += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                if (elapsedTime >= nextActionTime)
                {
                    actionByTime.Invoke(elapsedTime, totalTime);
                    nextActionTime += actionInterval;
                }

                yield return null;
            }

            callback?.Invoke();
        }

        private IEnumerator CO_WaitUntilConditionCoroutine(Func<bool> condition, Action action)
        {
            yield return new WaitUntil(condition);
            action.Invoke();
        }
        private IEnumerator CO_WaitUntilConditionCoroutine(Func<bool> condition, Action<float> actionOverTime, Action callback, bool useUnscaledTime)
        {
            float elapsedTime = 0f;

            while (!condition.Invoke())
            {
                elapsedTime += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                actionOverTime.Invoke(elapsedTime);
                yield return null;
            }

            callback?.Invoke();
        }
        private IEnumerator CO_WaitUntilConditionCoroutine(Func<bool> condition, Action<float> actionByTime, float actionInterval, Action callback, bool useUnscaledTime)
        {
            float elapsedTime = 0f;
            float nextActionTime = 0f;

            while (!condition.Invoke())
            {
                elapsedTime += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                if (elapsedTime >= nextActionTime)
                {
                    actionByTime.Invoke(elapsedTime);
                    nextActionTime += actionInterval;
                }

                yield return null;
            }

            callback?.Invoke();
        }

        private IEnumerator CO_WaitForEndOfFrameCoroutine(Action action)
        {
            yield return new WaitForEndOfFrame();
            action.Invoke();
        }

        private IEnumerator CO_WaitForFramesCoroutine(Action action, int frameDelayCount)
        {
            for (int i = 0; i < frameDelayCount; i++)
            {
                yield return null;
            }
            action.Invoke();
        }

        public Guid StartCoroutine(object owner, IEnumerator coroutine, float timeout = 0f, bool useUnscaledTime = false)
        {
            owner ??= this;
            if (!_activeCoroutines.ContainsKey(owner))
            {
                _activeCoroutines.Add(owner, new Dictionary<Guid, Coroutine>());
            }

            var id = Guid.NewGuid();

            if (timeout > 0)
            {
                StartCoroutine(CO_TryToStopCoroutineAfterTimeout(owner, timeout, id, useUnscaledTime));
            }

            var notifyCoroutineFinishedMethod = CO_NotifyCoroutineFinished(owner, coroutine, id);
            var coroutineWithFinishNotifier = StartCoroutine(notifyCoroutineFinishedMethod);
            _activeCoroutines[owner].Add(id, coroutineWithFinishNotifier);
            return id;
        }

        public void StartUniqueCoroutine(object owner, IEnumerator coroutine, string name, float timeout = 0f, bool useUnscaledTime = false)
        {
            owner ??= this;
            if (_activeUniqueCoroutines.TryGetValue(owner, out var coroutines))
            {
                if (coroutines.ContainsKey(name))
                {
                    Debug.LogError($"[CoroutineService] {coroutine} is already active with owner {owner}");
                    return;
                }
            }
            else
            {
                _activeUniqueCoroutines[owner] = new Dictionary<string, Coroutine>();
            }

            var notifyUniqueCoroutineFinishedMethod = CO_NotifyUniqueCoroutineFinished(owner, coroutine, name);
            var coroutineWithFinishNotifier = StartCoroutine(notifyUniqueCoroutineFinishedMethod);
            _activeUniqueCoroutines[owner].Add(name, coroutineWithFinishNotifier);

            if (timeout > 0)
            {
                StartCoroutine(CO_TryToStopUniqueCoroutineAfterTimeout(owner, timeout, name, useUnscaledTime));
            }
        }

        public bool IsCoroutineActive(object owner, string name)
        {
            if (_activeUniqueCoroutines.TryGetValue(owner ?? this, out var coroutines))
            {
                return coroutines.ContainsKey(name);
            }
            return false;
        }

        private IEnumerator CO_NotifyUniqueCoroutineFinished(object owner, IEnumerator originalCoroutine, string name)
        {
            yield return originalCoroutine;
            _activeUniqueCoroutines[owner ?? this].Remove(name);
        }

        private IEnumerator CO_NotifyCoroutineFinished(object owner, IEnumerator originalCoroutine, Guid coroutineId)
        {
            yield return originalCoroutine;
            _activeCoroutines[owner ?? this].Remove(coroutineId);
        }

        private IEnumerator CO_TryToStopCoroutineAfterTimeout(object owner, float seconds, Guid coroutineId, bool useUnscaledTime)
        {
            owner ??= this;
            yield return StartCoroutine(CO_WaitForSecondsCoroutine(() => { }, seconds, useUnscaledTime));
            if (!_activeCoroutines.TryGetValue(owner, out var coroutines))
            {
                yield break;
            }
            if (coroutines.ContainsKey(coroutineId))
            {
                StopCoroutine(owner, coroutineId);
            }
        }

        private IEnumerator CO_TryToStopUniqueCoroutineAfterTimeout(object owner, float seconds, string name, bool useUnscaledTime)
        {
            owner ??= this;
            yield return StartCoroutine(CO_WaitForSecondsCoroutine(() => { }, seconds, useUnscaledTime));
            if (!_activeUniqueCoroutines.TryGetValue(owner, out var coroutines))
            {
                yield break;
            }
            if (coroutines.ContainsKey(name))
            {
                StopUniqueCoroutine(owner, name);
            }
        }

        public void StopCoroutine(object owner, Guid coroutineId)
        {
            owner ??= this;
            if (_activeCoroutines.ContainsKey(owner) &&
               _activeCoroutines[owner].TryGetValue(coroutineId, out var coroutine))
            {
                StopCoroutine(coroutine);
                _activeCoroutines[owner].Remove(coroutineId);
            }
        }

        public void StopUniqueCoroutine(object owner, string name)
        {
            owner ??= this;
            if (_activeUniqueCoroutines.ContainsKey(owner) && _activeUniqueCoroutines.TryGetValue(owner, out var coroutines))
            {
                if (coroutines.TryGetValue(name, out var coroutineToStop))
                {
                    StopCoroutine(coroutineToStop);
                    _activeUniqueCoroutines[owner].Remove(name);
                }
            }
        }

        private void StopAllNonUniqueCoroutines(object owner)
        {
            owner ??= this;
            if (_activeCoroutines.TryGetValue(owner, out var coroutines))
            {
                foreach (var coroutine in coroutines.Values)
                {
                    StopCoroutine(coroutine);
                }

                _activeCoroutines[owner].Clear();
            }
        }

        private void StopAllUniqueCoroutinesFromOneOwner(object owner)
        {
            owner ??= this;
            if (_activeUniqueCoroutines.TryGetValue(owner, out var coroutines))
            {
                foreach (var coroutine in coroutines)
                {
                    StopCoroutine(coroutine.Value);
                }

                _activeUniqueCoroutines[owner].Clear();
            }
        }

        public void StopAllCoroutinesFromOneOwner(object owner)
        {
            owner ??= this;
            StopAllUniqueCoroutinesFromOneOwner(owner);
            StopAllNonUniqueCoroutines(owner);
        }

        public void Dispose()
        {
            foreach (var owner in _activeUniqueCoroutines.Keys)
            {
                StopAllUniqueCoroutinesFromOneOwner(owner);
            }

            foreach (var owner in _activeCoroutines.Keys)
            {
                StopAllNonUniqueCoroutines(owner);
            }

            _activeUniqueCoroutines.Clear();
            _activeCoroutines.Clear();
        }
    }
}