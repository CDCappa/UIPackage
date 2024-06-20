using UnityEngine;

namespace UIP.Runtime.Common.Extensions
{
    public static class GameObjectExtensions
    {
        public static T AddOrGetComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();

            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }

            return component;
        }
    }
}