using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Transformalize.Libs.Newtonsoft.Json.Linq;
using Transformalize.Libs.SemanticLogging;
using Transformalize.Libs.SemanticLogging.Sinks;
using Transformalize.Libs.SemanticLogging.Utility;
using Transformalize.Main;
using Guard = Transformalize.Libs.SemanticLogging.Utility.Guard;

namespace Transformalize.Logging {
    /// <summary>
    /// Sink that writes entries to a Elasticsearch server.
    /// </summary>
    public class ElasticSearchSink : IObserver<EventEntry>, IDisposable {
        private const string INDEX_NAME = "Transformalize";
        private const string TYPE_NAME = "LogEntry";

        private readonly BufferedEventPublisher<EventEntry> _bufferedPublisher;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly Log _log;

        private readonly Uri _elasticsearchUrl;
        private readonly string _processName;
        private readonly TimeSpan _onCompletedTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transformalize.Logging.ElasticSearchSink"/> class with the specified connection string and table address.
        /// </summary>
        /// <param name="processName">Transformalize process name.</param>
        /// <param name="log">The Transformalize log configuration.</param>
        /// <param name="bufferInterval">The buffering interval to wait for events to accumulate before sending them to Elasticsearch.</param>
        /// <param name="bufferingCount">The buffering event entry count to wait before sending events to Elasticsearch </param>
        /// <param name="maxBufferSize">The maximum number of entries that can be buffered while it's sending to Windows Azure Storage before the sink starts dropping entries.</param>
        /// <param name="onCompletedTimeout">Defines a timeout interval for when flushing the entries after an <see cref="OnCompleted"/> call is received and before disposing the sink.
        /// This means that if the timeout period elapses, some event entries will be dropped and not sent to the store. Normally, calling <see cref="IDisposable.Dispose"/> on 
        /// the <see cref="System.Diagnostics.Tracing.EventListener"/> will block until all the entries are flushed or the interval elapses.
        /// If <see langword="null"/> is specified, then the call will block indefinitely until the flush operation finishes.</param>
        public ElasticSearchSink(string processName, Log log, TimeSpan bufferInterval, int bufferingCount, int maxBufferSize, TimeSpan onCompletedTimeout) {
            Guard.ArgumentNotNull(log, "log");
            Guard.ArgumentIsValidTimeout(onCompletedTimeout, "onCompletedTimeout");
            Guard.ArgumentGreaterOrEqualThan(0, bufferingCount, "bufferingCount");

            _processName = processName;
            _onCompletedTimeout = onCompletedTimeout;

            _log = log;
            _elasticsearchUrl = new Uri(log.Connection.Uri() + "_bulk");
            var sinkId = string.Format(CultureInfo.InvariantCulture, "ElasticSearchSink ({0})", _processName);
            _bufferedPublisher = log.Async ?
                BufferedEventPublisher<EventEntry>.CreateAndStart(sinkId, PublishEventsAsync, bufferInterval, bufferingCount, maxBufferSize, _cancellationTokenSource.Token) :
                BufferedEventPublisher<EventEntry>.CreateAndStart(sinkId, PublishEvents, bufferInterval, bufferingCount, maxBufferSize, _cancellationTokenSource.Token);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="Transformalize.Libs.SemanticLogging.Sinks.ElasticsearchSink"/> class.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Notifies the observer that the provider has finished sending push-based notifications.
        /// </summary>
        public void OnCompleted() {
            FlushSafe();
            Dispose();
        }

        /// <summary>
        /// Provides the sink with new data to write.
        /// </summary>
        /// <param name="value">The current entry to write to Windows Azure.</param>
        public void OnNext(EventEntry value) {
            if (value == null) {
                return;
            }

            _bufferedPublisher.TryPost(value);
        }

        /// <summary>
        /// Notifies the observer that the provider has experienced an error condition.
        /// </summary>
        /// <param name="error">An object that provides additional information about the error.</param>
        public void OnError(Exception error) {
            FlushSafe();
            Dispose();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Transformalize.Libs.SemanticLogging.Sinks.ElasticsearchSink"/> class.
        /// </summary>
        ~ElasticSearchSink() {
            Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">A value indicating whether or not the class is disposing.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed",
            MessageId = "cancellationTokenSource", Justification = "Token is canceled")]
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                _cancellationTokenSource.Cancel();
                _bufferedPublisher.Dispose();
            }
        }

        internal async Task<int> PublishEventsAsync(IList<EventEntry> collection) {
            HttpClient client = null;

            try {
                client = new HttpClient();

                string logMessages;
                using (var serializer = new ElasticsearchEventEntrySerializer(INDEX_NAME, TYPE_NAME, _processName, false)) {
                    logMessages = serializer.Serialize(collection);
                }
                var content = new StringContent(logMessages);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.PostAsync(this._elasticsearchUrl, content, _cancellationTokenSource.Token).ConfigureAwait(false);

                // If there is an exception
                if (response.StatusCode != HttpStatusCode.OK) {
                    // Check the response for 400 bad request
                    if (response.StatusCode == HttpStatusCode.BadRequest) {
                        var messagesDiscarded = collection.Count();

                        var errorContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                        string serverErrorMessage;

                        // Try to parse the exception message
                        try {
                            var errorObject = JObject.Parse(errorContent);
                            serverErrorMessage = errorObject["error"].Value<string>();
                        } catch (Exception) {
                            // If for some reason we cannot extract the server error message log the entire response
                            serverErrorMessage = errorContent;
                        }

                        // We are unable to write the batch of event entries - Possible poison message
                        // I don't like discarding events but we cannot let a single malformed event prevent others from being written
                        // We might want to consider falling back to writing entries individually here
                        SemanticLoggingEventSource.Log.ElasticsearchSinkWriteEventsFailedAndDiscardsEntries(messagesDiscarded, serverErrorMessage);

                        return messagesDiscarded;
                    }

                    // This will leave the messages in the buffer
                    return 0;
                }

                var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var responseObject = JObject.Parse(responseString);

                var items = responseObject["items"] as JArray;

                // If the response return items collection
                if (items != null) {
                    // NOTE: This only works with Elasticsearch 1.0
                    // Alternatively we could query ES as part of initialization check results or fall back to trying <1.0 parsing
                    // We should also consider logging errors for individual entries
                    return items.Count(t => t["create"]["status"].Value<int>().Equals(201));

                    // Pre-1.0 Elasticsearch
                    // return items.Count(t => t["create"]["ok"].Value<bool>().Equals(true));
                }

                return 0;
            } catch (OperationCanceledException) {
                return 0;
            } catch (Exception ex) {
                // Although this is generally considered an anti-pattern this is not logged upstream and we have context
                SemanticLoggingEventSource.Log.ElasticsearchSinkWriteEventsFailed(ex.ToString());
                throw;
            } finally {
                if (client != null) {
                    client.Dispose();
                }
            }
        }

        internal Task<int> PublishEvents(IList<EventEntry> collection) {
            WebClient client = null;

            try {
                client = new WebClient();

                string logMessages;
                using (
                    var serializer = new ElasticsearchEventEntrySerializer(INDEX_NAME, TYPE_NAME, _processName, false)) {
                    logMessages = serializer.Serialize(collection);
                }
                client.Headers.Add(HttpResponseHeader.ContentType, "application/json");

                try {
                    var response = client.UploadData(_elasticsearchUrl, "POST", System.Text.Encoding.UTF8.GetBytes(logMessages));
                } catch (Exception ex) {
                    var messagesDiscarded = collection.Count;
                    SemanticLoggingEventSource.Log.ElasticsearchSinkWriteEventsFailedAndDiscardsEntries(messagesDiscarded, ex.Message);
                    return new Task<int>(() => messagesDiscarded);
                }

                return new Task<int>(() => collection.Count);
            } finally {
                if (client != null) client.Dispose();
            }
        }

        private void FlushSafe() {
            try {
                if (_log.Async) {
                    _bufferedPublisher.FlushAsync().Wait(_onCompletedTimeout);
                } else {
                    _bufferedPublisher.TriggerFlush();
                }
            } catch (AggregateException ex) {
                // Flush operation will already log errors. Never expose this exception to the observable.
                ex.Handle(e => e is FlushFailedException);
            }
        }
    }
}