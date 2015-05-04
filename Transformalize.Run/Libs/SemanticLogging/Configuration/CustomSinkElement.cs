// Copyright (c) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Xml.Linq;
using Transformalize.Run.Libs.SemanticLogging.Observable;
using Transformalize.Run.Libs.SemanticLogging.Utility;

namespace Transformalize.Run.Libs.SemanticLogging.Configuration
{
    internal class CustomSinkElement : ISinkElement
    {
        private readonly XName sinkName = XName.Get("customSink", Constants.Namespace);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated with Guard class")]
        public bool CanCreateSink(XElement element)
        {
            Guard.ArgumentNotNull(element, "element");

            return element.Name == this.sinkName;
        }

        public IObserver<EventEntry> CreateSink(XElement element)
        {
            Guard.ArgumentNotNull(element, "element");

            var subject = new EventEntrySubject();
            var sink = XmlUtil.CreateInstance<IObserver<EventEntry>>(element);
            subject.Subscribe(sink);
            return subject;
        }
    }
}
