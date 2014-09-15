using System;
using System.Linq.Expressions;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Extensions;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Mapping.SpecialFields
{
	[JsonConverter(typeof(ReadAsTypeConverter<TimestampFieldMapping>))]
	public interface ITimestampFieldMapping : ISpecialField
	{
		[JsonProperty("enabled")]
		bool Enabled { get; set; }

		[JsonProperty("path")]
		PropertyPathMarker Path { get; set; }

		[JsonProperty("format")]
		string Format { get; set; }
	}

	public class TimestampFieldMapping : ITimestampFieldMapping
	{
		public bool Enabled { get; set; }

		public PropertyPathMarker Path { get; set; }

		public string Format { get; set; }
	}


	public class TimestampFieldMappingDescriptor<T> : ITimestampFieldMapping
	{
		private ITimestampFieldMapping Self { get { return this; } }

		bool ITimestampFieldMapping.Enabled { get; set;}

		PropertyPathMarker ITimestampFieldMapping.Path { get; set;}

		string ITimestampFieldMapping.Format { get; set; }

		public TimestampFieldMappingDescriptor<T> Enabled(bool enabled = true)
		{
			Self.Enabled = enabled;
			return this;
		}
		public TimestampFieldMappingDescriptor<T> Path(string path)
		{
			Self.Path = path;
			return this;
		}
		public TimestampFieldMappingDescriptor<T> Path(Expression<Func<T, object>> objectPath)
		{
			objectPath.ThrowIfNull("objectPath");
			Self.Path = objectPath;
			return this;
		}
		public TimestampFieldMappingDescriptor<T> Format(string format)
		{
			Self.Format = format;
			return this;
		}

		
	}
}