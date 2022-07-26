using System;
using System.Collections.Generic;

namespace Assets.Scripts.Runtime.Logic
{
    public static class CollectionExtensions
    {
        public static IList<T> Shuffle<T>(this IEnumerable<T> input)
        {
            var resultList = new List<T>(input);

            int current = resultList.Count;
            var random = new Random();
            while (current > 1)
            {
                current--;
                int newPosition = random.Next(current + 1);
                (resultList[current], resultList[newPosition]) = (resultList[newPosition], resultList[current]);
            }
            return resultList;
        }
    }
}
