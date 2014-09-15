using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum FieldIndexOption
	{
		[EnumMember(Value = "analyzed")]
		Analyzed,
		[EnumMember(Value = "not_analyzed")]
		NotAnalyzed,
		[EnumMember(Value = "no")]
		No
	}
}
