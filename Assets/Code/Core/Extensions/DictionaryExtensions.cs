using System.Collections.Generic;

namespace UIPackage.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static TKey GetFirstKeyByValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value)
        {
            foreach (var kvp in dictionary)
            {
                if (EqualityComparer<TValue>.Default.Equals(kvp.Value, value))
                {
                    return kvp.Key;
                }
            }

            UnityEngine.Debug.LogWarning("Value not found in dictionary");

            return default(TKey);
        }

        public static List<TKey> GetKeysByValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TValue value)
        {
            List<TKey> keys = new List<TKey>();

            foreach (var kvp in dictionary)
            {
                if (EqualityComparer<TValue>.Default.Equals(kvp.Value, value))
                {
                    keys.Add(kvp.Key);
                }
            }

            if (keys.Count == 0)
            {
                UnityEngine.Debug.LogWarning("Value not found in dictionary");
            }

            return keys;
        }
    }
}