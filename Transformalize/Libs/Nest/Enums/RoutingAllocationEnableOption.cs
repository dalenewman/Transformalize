using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum RoutingAllocationEnableOption
	{
		[EnumMember(Value = "all")]
		All,
		[EnumMember(Value = "primaries")]
		Primaries,
		[EnumMember(Value = "new_primaries")]
		NewPrimaries,
		[EnumMember(Value = "none")]
		None
	}
}
