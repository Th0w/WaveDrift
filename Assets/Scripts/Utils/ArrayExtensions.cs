using System;
using System.Collections.Generic;
using System.Linq;

public static class ArrayExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
        if (enumerable.Count() == 0) { return; }

        foreach (var entity in enumerable) {
            action(entity);
        }
    }

    public static T Random<T>(this IEnumerable<T> enumerable) {
        int cnt = enumerable.Count();
        if (cnt == 0) {
            return default(T);
        } else {
            return enumerable.ElementAt(UnityEngine.Random.Range(0, cnt));
        }
    }
}

public static class Utils
{
    public static void ForLoop(int max, Action<int> action) {
        int i;
        for (i = 0; i < max; ++i) { action(i); }
    }
}