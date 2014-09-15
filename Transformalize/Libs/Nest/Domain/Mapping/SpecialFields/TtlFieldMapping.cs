using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Mapping.SpecialFields
{
	[JsonConverter(typeof(ReadAsTypeConverter<TtlFieldMapping>))]
	public interface ITtlFieldMapping : ISpecialField
	{
		[JsonProperty("enabled")]
		bool? Enabled { get; set; }

		[JsonProperty("default")]
		string Default { get; set; }
	}

	public class TtlFieldMapping : ITtlFieldMapping
	{
		public bool? Enabled { get; set; }
		public string Default { get; set; }
	}

	public class TtlFieldMappingDescriptor : ITtlFieldMapping
	{
		private ITtlFieldMapping Self { get { return this; } }

		bool? ITtlFieldMapping.Enabled { get; set; }

		string ITtlFieldMapping.Default { get; set; }

		public TtlFieldMappingDescriptor Enable(bool enable = true)
		{
			Self.Enabled = enable;
			return this;
		}
		public TtlFieldMappingDescriptor Default(string defaultTtl)
		{
			Self.Default = defaultTtl;
			return this;
		}
	}
}