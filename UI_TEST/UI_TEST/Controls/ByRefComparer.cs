using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace UI_TEST
{
    public sealed class ByRefComparer<T> : IEqualityComparer<T>
    {
        public static readonly IEqualityComparer<T> Default = new ByRefComparer<T>();

        public bool Equals(T x, T y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}