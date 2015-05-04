using System;
using Transformalize.Configuration;
using Transformalize.Main;
using Transformalize.Run.Libs.SemanticLogging;

namespace Transformalize.Run.Logging {

    public static class EmailSinkExtensions {
        public static SinkSubscription<EmailSink> LogToEmail(
        this IObservable<EventEntry> eventStream, TflLog log) {
            var sink = new EmailSink(log);
            var subscription = eventStream.Subscribe(sink);
            return new SinkSubscription<EmailSink>(subscription, sink);
        }
    }
}
