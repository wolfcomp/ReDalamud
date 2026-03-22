using System.Numerics;
using YamlDotNet.Core.Tokens;

namespace ReDalamud.Shared;

public class FastLookupList<T> : List<T>
{
    public T FindFromNeedle<V>(Func<T, V> needleValue, Func<T, T, (bool, bool)> checkValue, V searchValue) where V : INumber<V>
    {
        var lower = 0;
        var upper = Count - 1;
        if (lower == upper) return this[0];
        while (lower != upper - 1)
        {
            if (upper - lower <= 50)
            {
                for(var i = lower; i < upper; i ++)
                {
                    if (checkValue(this[i], this[i]).Item1)
                    {
                        return this[i];
                    }
                }
                return this[lower];
            }

            var middleIndex = (upper - lower) / 2 + lower;
            if (needleValue(this[middleIndex]) > searchValue)
                upper = middleIndex;
            else
                lower = middleIndex;
        }
        return checkValue(this[lower], this[upper]).Item2 ? this[lower] : this[upper];
    }
}