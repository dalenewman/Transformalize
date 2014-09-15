using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.DSL.Facets
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum TermsOrder
	{
		[EnumMember(Value = "count")]
		Count = 0,
		[EnumMember(Value = "term")]
		Term,
		[EnumMember(Value = "reverse_count")]
		ReverseCount,
		[EnumMember(Value = "reverse_term")]
		ReverseTerm
	}
}
