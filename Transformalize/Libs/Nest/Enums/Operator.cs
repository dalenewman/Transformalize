using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum Operator
	{
		[EnumMember(Value = "and")]
		And,
		[EnumMember(Value = "or")]
		Or
	}
}
