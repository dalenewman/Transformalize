using System;
using System.Linq.Expressions;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Mapping.SpecialFields
{
	[JsonConverter(typeof(ReadAsTypeConverter<AnalyzerFieldMapping>))]
	public interface IAnalyzerFieldMapping : ISpecialField
	{
		[JsonProperty("index"), JsonConverter(typeof(YesNoBoolConverter))]
		bool? Index { get; set; }

		[JsonProperty("path")]
		PropertyPathMarker Path { get; set; }
	}

	public class AnalyzerFieldMapping : IAnalyzerFieldMapping
	{
		public bool? Index { get; set; }

		public PropertyPathMarker Path { get; set; }
	}


	public class AnalyzerFieldMappingDescriptor<T> : IAnalyzerFieldMapping
	{
		private IAnalyzerFieldMapping Self { get { return this; } }

		bool? IAnalyzerFieldMapping.Index { get; set; }

		PropertyPathMarker IAnalyzerFieldMapping.Path { get; set; }

		public AnalyzerFieldMappingDescriptor<T> Index(bool indexed = true)
		{
			Self.Index = indexed;
			return this;
		}
		public AnalyzerFieldMappingDescriptor<T> Path(string path)
		{
			Self.Path = path;
			return this;
		}
		public AnalyzerFieldMappingDescriptor<T> Path(Expression<Func<T, object>> objectPath)
		{
			Self.Path = objectPath;
			return this;
		}
	}
}