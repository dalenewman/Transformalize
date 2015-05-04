using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace Transformalize.Run.Logging {

    [EventSource(Name = "Transformalize")]
    public class TflEventSource : EventSource {

        private static readonly Lazy<TflEventSource> Instance = new Lazy<TflEventSource>(() => new TflEventSource());
        public static TflEventSource Log { get { return Instance.Value; } }

        private TflEventSource() { }

        //TODO: Temporary implementation

        public class Tasks {
            public const EventTask Input = (EventTask)1;
            public const EventTask Output = (EventTask)2;
        }

        [Event(1, Message = "{0}", Level = EventLevel.Informational)]
        public void Info(string message, string process, string entity) {
            if (this.IsEnabled()) {
                this.WriteEvent(1, message, process, entity);
            }
        }

        [Event(2, Message = "{0}", Level = EventLevel.Verbose)]
        public void Debug(string message, string process, string entity) {
            if (this.IsEnabled()) {
                this.WriteEvent(2, message, process, entity);
            }
        }

        [Event(3, Message = "{0}", Level = EventLevel.Warning)]
        public void Warn(string message, string process, string entity) {
            if (this.IsEnabled()) {
                this.WriteEvent(3, message, process, entity);
            }
        }

        [Event(4, Message = "{0}", Level = EventLevel.Error)]
        public void Error(string message, string process, string entity) {
            if (this.IsEnabled()) {
                this.WriteEvent(4, message, process, entity);
            }
        }

        public void Update() {
            SendCommand(this, EventCommand.Update, new Dictionary<string, string>());
        }
    }
}
