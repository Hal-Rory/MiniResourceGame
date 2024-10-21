using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Common.Utility
{
    public static class ListExtensions
    {
        public static T GetRandomIndex<T>(this IList<T> collection)
        {
            return collection[Random.Range(0, collection.Count)];
        }

        public static T GetRandomIndex<T>(this IEnumerable<T> collection, int count)
        {
            int random = Random.Range(0, count);
            return collection.ElementAt(random);
        }

        public static IList<T> GetRandomCollection<T>(this IList<T> collection, int count)
        {
            T[] randomCollection = new T[count];
            List<T> tempCollection = new List<T>(collection);
            for (int c = 0; c < count; c++)
            {
                randomCollection[c] = tempCollection.GetRandomIndex();
                tempCollection.Remove(randomCollection[c]);
            }
            return randomCollection;
        }
    }
}