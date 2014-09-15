using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum NonStringIndexOption
	{
		[EnumMember(Value = "no")]
		No,
		[EnumMember(Value = "analyzed")]
		Analyzed,
		[EnumMember(Value = "not_analyzed")]
		NotAnalyzed 
	}
}
