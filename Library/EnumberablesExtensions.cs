using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace System
{
    /// <summary>
    /// Wetnap extension methods for enumerable types like array and list.
    /// </summary>
    partial class WetnapExtensions
    {
        #region IEnumerable<>

        /// <summary>
        /// Indicates if the sequence has no elements.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsEmpty<T>(this IEnumerable<T> source)
        {
            return !HasItems(source);
        }

        /// <summary>
        /// Indicates if the sequence has at least one element.  Opposite of IsEmpty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool HasItems<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return false;
            }

            if ( source is IList || source is IQueryable )
            {
                return source.Count() > 0;
            }

            using (var enumerator = source.GetEnumerator())
            {
                return enumerator.MoveNext();
            }
        }

        #endregion

        #region IList<>

        /// <summary>
        /// Indicates whether there is an element at the given index.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IsIndexInBounds(this IList list, int index)
        {
            if (list == null)
                return false;
            return index >= 0 && index < list.Count;
        }

        /// <summary>
        /// Removes all elements from the list that satisfy the condition.  Returns the list that was passed in (minus removed elements) for chaining.  This is the same as List's RemoveAll but can be used for any IList object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="match"></param>
        /// <returns></returns>
        public static IList<T> RemoveAll<T>(this IList<T> list, Predicate<T> condition)
        { 
            if (list == null || list.Count == 0)
                return list;
            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                if (condition(item))
                {
                    list.RemoveAt(i);
                    i--;
                }
            }
            return list;
        }

        #endregion

        #region ISet

        /// <summary>
        /// Adds a bunch of items to the set at once.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="items"></param>
        public static void Add<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                set.Add(item);
            }
        }

        /// <summary>
        /// Removes a bunch of items from the set.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="set"></param>
        /// <param name="items"></param>
        public static void Remove<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                set.Remove(item);
            }
        }

        #endregion

        #region Dictionary

        /// <summary>
        /// Returns the value for the given key.  If it doesn't exist, the createIfMissing method will be invoked and will be added to the dictionay.
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="set"></param>
        /// <param name="key"></param>
        /// <param name="createIfMissing"></param>
        public static TValue Ensure<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> createIfMissing)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = createIfMissing();
                dictionary.Add(key, value);
            }
            return value;
        }

        /// <summary>
        /// Returns the value for the given key.  If it doesn't exist, the createIfMissing method will be invoked and will be added to the dictionay.
        /// </summary>
        public static object Ensure(this IDictionary dictionary, object key, Func<object> createIfMissing)
        {
            object value;
            if (dictionary.Contains(key))
            {
                value = dictionary[key];
            }
            else
            {
                value = createIfMissing();
                dictionary.Add(key, value);
            }
            return value;
        }

        #endregion

    }

}
