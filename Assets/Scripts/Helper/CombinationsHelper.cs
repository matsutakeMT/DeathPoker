
using System.Collections.Generic;
using System.Linq;

public static class CombinationsHelper
{
    public static IEnumerable<T[]> Combinations<T>(this IEnumerable<T> source, int r)
    {
        var arr = source.ToArray();
        int n = arr.Length;
        int[] indices = Enumerable.Range(0, r).ToArray();

        while (true)
        {
            yield return indices.Select(i => arr[i]).ToArray();

            int pos = r - 1;
            while (pos >= 0 && indices[pos] == n - r + pos) pos--;
            if (pos < 0) yield break;

            indices[pos]++;
            for (int i = pos + 1; i < r; i++)
                indices[i] = indices[pos] + (i - pos);
        }
    }
}