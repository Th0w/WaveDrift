using System;
using System.Collections.Generic;
using System.Linq;

public static class ArrayExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        if (enumerable.Count() == 0) { return; }

        foreach (var entity in enumerable)
        {
            action(entity);
        }
    }
}