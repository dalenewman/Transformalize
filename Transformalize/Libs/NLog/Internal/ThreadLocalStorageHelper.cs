#region License
// /*
// See license included in this library folder.
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