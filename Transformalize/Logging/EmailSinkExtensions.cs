using System;
using Transformalize.Libs.EnterpriseLibrary.SemanticLogging;
using Transformalize.Main;
using Transformalize.Main.Providers.Mail;

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
