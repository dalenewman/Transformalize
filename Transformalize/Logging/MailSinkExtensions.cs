using System;
using System.Collections.Generic;
using Transformalize.Libs.SemanticLogging;

namespace Transformalize.Logging {

    public static class MemorySinkExtensions {

        public static SinkSubscription<MemorySink> LogToMemory(
        this IObservable<EventEntry> eventStream, ref SynchronizedCollection<string> log) {
            var sink = new MemorySink(ref log);
            var subscription = eventStream.Subscribe(sink);
            return new SinkSubscription<MemorySink>(subscription, sink);
        }
    }
}
