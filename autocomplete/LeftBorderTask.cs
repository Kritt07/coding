using System;
using System.Collections.Generic;
using System.Linq;

namespace Autocomplete;

public class LeftBorderTask
{
    /// <returns>
    /// Возвращает индекс левой границы.
    /// То есть индекс максимальной фразы, которая не начинается с prefix и меньшая prefix.
    /// Если такой нет, то возвращает -1
    /// </returns>
    /// <remarks>
    /// Функция должна быть рекурсивной
    /// и работать за O(log(items.Length)*L), где L — ограничение сверху на длину фразы
    /// </remarks>
    public static int GetLeftBorderIndex(IReadOnlyList<string> phrases, string prefix, int left, int right)
    {
        if (left + 1 >= right) 
            return left;
            
        var middle = left + (right - left) / 2;
        
        if (string.Compare(phrases[middle], prefix, StringComparison.InvariantCultureIgnoreCase) < 0 
            && !phrases[middle].StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
            return GetLeftBorderIndex(phrases, prefix, middle, right);
		return GetLeftBorderIndex(phrases, prefix, left, middle);
    }
}