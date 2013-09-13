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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Transformalize.Libs.NLog.LogReceiverService
{
#if WCF_SUPPORTED
    using System.Runtime.Serialization;
#endif

    /// <summary>
    ///     Wire format for NLog Event.
    /// </summary>
#if WCF_SUPPORTED
    [DataContract(Name = "e", Namespace = LogReceiverServiceConfig.WebServiceNamespace)]
#endif
    [XmlType(Namespace = LogReceiverServiceConfig.WebServiceNamespace)]
#if !NET_CF
    [DebuggerDisplay("Event ID = {Id} Level={LevelName} Values={Values.Count}")]
#endif
    public class NLogEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NLogEvent" /> class.
        /// </summary>
        public NLogEvent()
        {
            ValueIndexes = new List<int>();
        }

        /// <summary>
        ///     Gets or sets the client-generated identifier of the event.
        /// </summary>
#if WCF_SUPPORTED
        [DataMember(Name = "id", Order = 0)]
#endif
        [XmlElement("id", Order = 0)]
        public int Id { get; set; }

        /// <summary>
        ///     Gets or sets the ordinal of the log level.
        /// </summary>
#if WCF_SUPPORTED
        [DataMember(Name = "lv", Order = 1)]
#endif
        [XmlElement("lv", Order = 1)]
        public int LevelOrdinal { get; set; }

        /// <summary>
        ///     Gets or sets the logger ordinal (index into <see cref="NLogEvents.Strings" />.
        /// </summary>
        /// <value>The logger ordinal.</value>
#if WCF_SUPPORTED
        [DataMember(Name = "lg", Order = 2)]
#endif
        [XmlElement("lg", Order = 2)]
        public int LoggerOrdinal { get; set; }

        /// <summary>
        ///     Gets or sets the time delta (in ticks) between the time of the event and base time.
        /// </summary>
#if WCF_SUPPORTED
        [DataMember(Name = "ts", Order = 3)]
#endif
        [XmlElement("ts", Order = 3)]
        public long TimeDelta { get; set; }

        /// <summary>
        ///     Gets or sets the message string index.
        /// </summary>
#if WCF_SUPPORTED
        [DataMember(Name = "m", Order = 4)]
#endif
        [XmlElement("m", Order = 4)]
        public int MessageOrdinal { get; set; }

        /// <summary>
        ///     Gets or sets the collection of layout values.
        /// </summary>
#if WCF_SUPPORTED
        [DataMember(Name = "val", Order = 100)]
#endif
        [XmlElement("val", Order = 100)]
        public string Values
        {
            get
            {
                var sb = new StringBuilder();
                var separator = string.Empty;

                if (ValueIndexes != null)
                {
                    foreach (var index in ValueIndexes)
                    {
                        sb.Append(separator);
                        sb.Append(index);
                        separator = "|";
                    }
                }

                return sb.ToString();
            }

            set
            {
                if (ValueIndexes != null)
                {
                    ValueIndexes.Clear();
                }
                else
                {
                    ValueIndexes = new List<int>();
                }

                if (!string.IsNullOrEmpty(value))
                {
                    var chunks = value.Split('|');

                    foreach (var chunk in chunks)
                    {
                        ValueIndexes.Add(Convert.ToInt32(chunk, CultureInfo.InvariantCulture));
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the collection of indexes into <see cref="NLogEvents.Strings" /> array for each layout value.
        /// </summary>
#if WCF_SUPPORTED
        [IgnoreDataMember]
#endif
        [XmlIgnore]
        internal IList<int> ValueIndexes { get; private set; }

        /// <summary>
        ///     Converts the <see cref="NLogEvent" /> to <see cref="LogEventInfo" />.
        /// </summary>
        /// <param name="context">
        ///     The <see cref="NLogEvent" /> object this <see cref="NLogEvent" /> is part of..
        /// </param>
        /// <param name="loggerNamePrefix">The logger name prefix to prepend in front of the logger name.</param>
        /// <returns>
        ///     Converted <see cref="LogEventInfo" />.
        /// </returns>
        internal LogEventInfo ToEventInfo(NLogEvents context, string loggerNamePrefix)
        {
            var result = new LogEventInfo(LogLevel.FromOrdinal(LevelOrdinal), loggerNamePrefix + context.Strings[LoggerOrdinal], context.Strings[MessageOrdinal]);
            result.TimeStamp = new DateTime(context.BaseTimeUtc + TimeDelta, DateTimeKind.Utc).ToLocalTime();
            for (var i = 0; i < context.LayoutNames.Count; ++i)
            {
                var layoutName = context.LayoutNames[i];
                var layoutValue = context.Strings[ValueIndexes[i]];

                result.Properties[layoutName] = layoutValue;
            }

            return result;
        }
    }
}