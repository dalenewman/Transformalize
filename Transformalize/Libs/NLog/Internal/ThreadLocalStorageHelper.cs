#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Threading;

#if SILVERLIGHT || NET_CF
#define TLS_WORKAROUND
#endif

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Helper for dealing with thread-local storage.
    /// </summary>
    internal static class ThreadLocalStorageHelper
    {
#if TLS_WORKAROUND
        private static int nextSlotNumber;
        private static WeakThreadReferenceWithData threadData;
#endif

        /// <summary>
        ///     Allocates the data slot for storing thread-local information.
        /// </summary>
        /// <returns>Allocated slot key.</returns>
        public static object AllocateDataSlot()
        {
#if TLS_WORKAROUND
            return Interlocked.Increment(ref nextSlotNumber);
#else
            return Thread.AllocateDataSlot();
#endif
        }

        /// <summary>
        ///     Gets the data for a slot in thread-local storage.
        /// </summary>
        /// <typeparam name="T">Type of the data.</typeparam>
        /// <param name="slot">The slot to get data for.</param>
        /// <returns>
        ///     Slot data (will create T if null).
        /// </returns>
        public static T GetDataForSlot<T>(object slot)
            where T : class, new()
        {
#if TLS_WORKAROUND
            IDictionary<int, object> dict = GetThreadDataDictionary(Thread.CurrentThread);
            int slotNumber = (int)slot;
            object v;
            if (!dict.TryGetValue(slotNumber, out v))
            {
                v = new T();
                dict.Add(slotNumber, v);
            }

            return (T)v;
#else
            var localDataStoreSlot = (LocalDataStoreSlot) slot;
            var v = Thread.GetData(localDataStoreSlot);
            if (v == null)
            {
                v = new T();
                Thread.SetData(localDataStoreSlot, v);
            }

            return (T) v;
#endif
        }

#if TLS_WORKAROUND
        private static IDictionary<int, object> GetThreadDataDictionary(Thread thread)
        {
            for (var trd = threadData; trd != null; trd = trd.Next)
            {
                var t = trd.ThreadReference.Target as Thread;
                if (t == thread)
                {
                    return trd.Data;
                }
            }

            var data = new Dictionary<int, object>();
            var wtr = new WeakThreadReferenceWithData();
            wtr.ThreadReference = new WeakReference(thread);
            wtr.Data = data;

            WeakThreadReferenceWithData oldThreadData;

            do
            {
                oldThreadData = threadData;
                wtr.Next = oldThreadData;
            }
            while (Interlocked.CompareExchange(ref threadData, wtr, oldThreadData) != oldThreadData);

            return data;
        }

        internal class WeakThreadReferenceWithData
        {
            internal WeakReference ThreadReference { get; set; }

            internal IDictionary<int, object> Data { get; set; }

            internal WeakThreadReferenceWithData Next { get; set; }
        }
#endif
    }
}