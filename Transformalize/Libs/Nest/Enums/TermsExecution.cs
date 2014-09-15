using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum TermsExecution
	{
		[EnumMember(Value = "plain")]
		Plain,
		[EnumMember(Value = "bool")]
		Bool,
		[EnumMember(Value = "and")]
		And,
		[EnumMember(Value = "or")]
		Or,
		[EnumMember(Value = "fielddata")]
		FieldData
	}
}
