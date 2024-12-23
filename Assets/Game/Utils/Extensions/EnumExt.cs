using System;
using System.Linq;

namespace Game.Utils
{
    public static class EnumExt
    {
        public static T[] GetValues<T>() where T : struct 
        {
            return (T[]) Enum.GetValues(typeof(T));
        }
    }
}