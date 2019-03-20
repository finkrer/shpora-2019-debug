using System;
using System.Collections.Generic;
using System.Linq;

namespace JPEG.Utilities
{
	static class IEnumerableExtensions
	{
		public static T MinOrDefault<T>(this IEnumerable<T> enumerable, Func<T, int> selector)
        {
            var enumerable1 = enumerable as T[] ?? enumerable.ToArray();
            var minValue = int.MaxValue;
            T min = default;
            foreach (var t in enumerable1)
            {
                var value = selector(t);
                if (value >= minValue) continue;
                minValue = value;
                min = t;
            }

            return min;
		}
    }
}