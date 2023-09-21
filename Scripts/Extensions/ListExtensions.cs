using System.Collections.Generic;
using System.Linq;

namespace Crimsilk.Utilities.Extensions
{
    public static class ListExtensions
    {
        public static T PopAt<T>(this IList<T> list, int index)
        {
            T item = list[index];
            list.RemoveAt(index);
            return item;
        }
        
        public static T PopFirst<T>(this IList<T> list)
        {
            T item = list.First();
            list.Remove(item);
            return item;
        }
        
        public static T PopLast<T>(this IList<T> list)
        {
            T item = list.Last();
            list.Remove(item);
            return item;
        }
    }
}