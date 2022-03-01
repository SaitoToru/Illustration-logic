using System;
using System.Collections.Generic;
using System.Linq;

public static class IEnumerableExtensions
{
    private sealed class CommonSelector<T, TKey> : IEqualityComparer<T>
    {
        private Func<T, TKey> m_selector;

        public CommonSelector(Func<T, TKey> selector)
        {
            m_selector = selector;
        }

        public bool Equals(T x, T y)
        {
            return m_selector(x).Equals(m_selector(y));
        }

        public int GetHashCode(T obj)
        {
            return m_selector(obj).GetHashCode();
        }
    }

    public static IEnumerable<T> Distinct<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> selector
    )
    {
        return source.Distinct(new CommonSelector<T, TKey>(selector));
    }
}