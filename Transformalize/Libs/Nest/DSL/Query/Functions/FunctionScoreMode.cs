using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.DSL.Query.Functions
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum FunctionScoreMode
	{
		[EnumMember(Value = "multiply")]
		Multiply,
		[EnumMember(Value = "sum")]
		Sum,
		[EnumMember(Value = "avg")]
		Average,
		[EnumMember(Value = "first")]
		First,
		[EnumMember(Value = "max")]
		Max,
		[EnumMember(Value = "min")]
		Min
	}
}