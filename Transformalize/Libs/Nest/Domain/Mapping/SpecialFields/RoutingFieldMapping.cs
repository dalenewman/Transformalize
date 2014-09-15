using System;
using System.Linq.Expressions;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Mapping.SpecialFields
{
	[JsonConverter(typeof(ReadAsTypeConverter<RoutingFieldMapping>))]
	public interface IRoutingFieldMapping : ISpecialField
	{
		[JsonProperty("required")]
		bool? Required { get; set; }

		[JsonProperty("path")]
		PropertyPathMarker Path { get; set; }
	}

	public class RoutingFieldMapping : IRoutingFieldMapping
	{
		public bool? Required { get; set; }

		public PropertyPathMarker Path { get; set; }
	}


	public class RoutingFieldMappingDescriptor<T> : IRoutingFieldMapping
	{
		private IRoutingFieldMapping Self { get { return this; } }

		bool? IRoutingFieldMapping.Required { get; set;}

		PropertyPathMarker IRoutingFieldMapping.Path { get; set; }

		public RoutingFieldMappingDescriptor<T> Required(bool required = true)
		{
			Self.Required = required;
			return this;
		}
		public RoutingFieldMappingDescriptor<T> Path(string path)
		{
			Self.Path = path;
			return this;
		}
		public RoutingFieldMappingDescriptor<T> Path(Expression<Func<T, object>> objectPath)
		{
			Self.Path = objectPath;
			return this;
		}

	}
}