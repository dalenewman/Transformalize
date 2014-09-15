using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum GeoOptimizeBBox
	{
		[EnumMember(Value = "memory")]
		Memory,
		[EnumMember(Value = "indexed")]
		Indexed,
		[EnumMember(Value = "none")]
		None
	}
}
