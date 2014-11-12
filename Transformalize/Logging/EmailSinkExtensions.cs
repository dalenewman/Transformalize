using System;
using Transformalize.Libs.SemanticLogging;
using Transformalize.Main;

namespace Transformalize.Logging {

    public static class EmailSinkExtensions {
        public static SinkSubscription<EmailSink> LogToEmail(
        this IObservable<EventEntry> eventStream, Log log) {
            var sink = new EmailSink(log);
            var subscription = eventStream.Subscribe(sink);
            return new SinkSubscription<EmailSink>(subscription, sink);
        }
    }
}
