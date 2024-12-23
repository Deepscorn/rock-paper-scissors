using UnityEngine;

namespace Game.Utils
{
    public static class GameObjectExt
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T result = go.GetComponent<T>();
            if ((System.Object)result == null) // Check that component exist. But do not add new if component exist, but destroyed
            {
                result = go.AddComponent<T>();
            }
            return result;
        }
    }
}