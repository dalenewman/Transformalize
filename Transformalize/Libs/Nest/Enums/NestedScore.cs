using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum NestedScore
	{
		[EnumMember(Value = "avg")]
		Average,
		[EnumMember(Value = "total")]
		Total,
		[EnumMember(Value = "max")]
		Max,
		[EnumMember(Value = "none")]
		None
	}
}
