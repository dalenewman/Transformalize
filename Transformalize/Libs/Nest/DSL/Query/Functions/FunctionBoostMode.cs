using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.DSL.Query.Functions
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum FunctionBoostMode
	{
		[EnumMember(Value = "multiply")]
		Multiply,
		[EnumMember(Value = "replace")]
		Replace,
		[EnumMember(Value = "sum")]
		Sum,
		[EnumMember(Value = "avg")]
		Average,
		[EnumMember(Value = "max")]
		Max,
		[EnumMember(Value = "min")]
		Min
	}
}