using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Domain.DSL
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ChildScoreType
	{
		[EnumMember(Value = "none")]
		None,
		[EnumMember(Value = "avg")]
		Average,
		[EnumMember(Value = "sum")]
		Sum,
		[EnumMember(Value = "max")]
		Max
	}
}
