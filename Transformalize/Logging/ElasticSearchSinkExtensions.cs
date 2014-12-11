using System;
using System.Threading;
using Transformalize.Libs.SemanticLogging;
using Transformalize.Libs.SemanticLogging.Utility;
using Transformalize.Main;

namespace Transformalize.Logging {
    /// <summary>
    /// Factories and helpers for using the <see cref="Transformalize.Logging.ElasticSearchSink"/>.
    /// </summary>
    public static class ElasticSearchSinkExtensions {
        /// <summary>
        /// Subscribes to an <see cref="IObservable{EventEntry}" /> using a <see cref="Transformalize.Logging.ElasticSearchSink" />.
        /// </summary>
        /// <param name="eventStream">The event stream. Typically this is an instance of <see cref="ObservableEventListener" />.</param>
        /// <param name="instanceName">The name of the instance originating the entries.</param>
        /// <param name="log">Transformalize log configuration.</param>
        /// <param name="bufferingInterval">The buffering interval between each batch publishing. Default value is <see cref="Buffering.DefaultBufferingInterval" />.</param>
        /// <param name="onCompletedTimeout">Defines a timeout interval for when flushing the entries after an <see cref="Libs.SemanticLogging.Sinks.ElasticsearchSink.OnCompleted" /> call is received and before disposing the sink.</param>
        /// <param name="bufferingCount">Buffering count to send entries sot Elasticsearch. Default value is <see cref="Buffering.DefaultBufferingCount" /></param>
        /// <param name="maxBufferSize">The maximum number of entries that can be buffered while it's sending to Elasticsearch before the sink starts dropping entries.
        /// This means that if the timeout period elapses, some event entries will be dropped and not sent to the store. Normally, calling <see cref="IDisposable.Dispose" /> on
        /// the <see cref="System.Diagnostics.Tracing.EventListener" /> will block until all the entries are flushed or the interval elapses.
        /// If <see langword="null" /> is specified, then the call will block indefinitely until the flush operation finishes.</param>
        /// <returns>
        /// A subscription to the sink that can be disposed to unsubscribe the sink and dispose it, or to get access to the sink instance.
        /// </returns>
        public static SinkSubscription<ElasticSearchSink> LogToElasticSearch(this IObservable<EventEntry> eventStream,
            string instanceName, Log log, TimeSpan? bufferingInterval = null, TimeSpan? onCompletedTimeout = null,
            int bufferingCount = Buffering.DefaultBufferingCount,
            int maxBufferSize = Buffering.DefaultMaxBufferSize) {
            var sink = new ElasticSearchSink(instanceName, log,
                bufferingInterval ?? Buffering.DefaultBufferingInterval,
                bufferingCount,
                maxBufferSize,
                onCompletedTimeout ?? Timeout.InfiniteTimeSpan);

            var subscription = eventStream.Subscribe(sink);
            return new SinkSubscription<ElasticSearchSink>(subscription, sink);
        }

    }
}