//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Utility
{
    /// <summary>
    /// Some utility extensions on <see cref="IEnumerable{T}"/> to suppliment
    /// those available from Linq.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Execute <paramref name="action"/> for each element of <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="T">Type of items in <paramref name="sequence"/>.</typeparam>
        /// <param name="sequence">Sequence of items to act on.</param>
        /// <param name="action">Action to invoke for each item.</param>
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach(var item in sequence)
            {
                action(item);
            }
        }

        /// <summary>
        /// Given a sequence, combine it with another sequence, passing the corresponding
        /// elements of each sequence to the <paramref name="zipper"/> action to create
        /// a new single value from the two sequence elements. "Zip" here refers to a zipper,
        /// not the compression algorithm. The resulting sequence will have the same number
        /// of elements as the shorter of sequence1 and sequence2.
        /// </summary>
        /// <typeparam name="T1">Type of the elments in the first sequence.</typeparam>
        /// <typeparam name="T2">Type of the elements in the second sequence.</typeparam>
        /// <typeparam name="TResult">Type of the resulting sequence elements.</typeparam>
        /// <param name="sequence1">The first sequence to combine.</param>
        /// <param name="sequence2">The second sequence to combine.</param>
        /// <param name="zipper">Func used to calculate the resulting values.</param>
        /// <returns>The result sequence.</returns>
        public static IEnumerable<TResult> Zip<T1, T2, TResult>(this IEnumerable<T1> sequence1,
                                                                IEnumerable<T2> sequence2, Func<T1, T2, TResult> zipper)
        {
            IEnumerator<T1> enumerator1 = sequence1.GetEnumerator();
            IEnumerator<T2> enumerator2 = sequence2.GetEnumerator();

            while (enumerator1.MoveNext())
            {
                if (!enumerator2.MoveNext())
                    yield break;

                yield return zipper(enumerator1.Current, enumerator2.Current);
            }
        }

        /// <summary>
        /// Take two sequences and return a new sequence of <see cref="KeyValuePair{TKey,TValue}"/> objects.
        /// </summary>
        /// <typeparam name="T1">Type of objects in sequence1.</typeparam>
        /// <typeparam name="T2">Type of objects in sequence2.</typeparam>
        /// <param name="sequence1">First sequence.</param>
        /// <param name="sequence2">Second sequence.</param>
        /// <returns>The sequence of <see cref="KeyValuePair{TKey,TValue}"/> objects.</returns>
        public static IEnumerable<KeyValuePair<T1, T2>> Zip<T1, T2>(this IEnumerable<T1> sequence1, IEnumerable<T2> sequence2)
        {
            return sequence1.Zip(sequence2, (i1, i2) => new KeyValuePair<T1, T2>(i1, i2));
        }

        /// <summary>
        /// Take two sequences and return a <see cref="IDictionary{TKey,TValue}"/> with the first sequence
        /// holding the keys and the corresponding elements of the second sequence containing the values.
        /// </summary>
        /// <typeparam name="TKey">Type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">Type of values in the dictionary.</typeparam>
        /// <param name="keys">Sequence of dictionary keys.</param>
        /// <param name="values">Sequence of dictionary values.</param>
        /// <returns>The constructed dictionary.</returns>
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            return keys.Zip(values).ToDictionary(p => p.Key, p => p.Value);
        }
    }
}
