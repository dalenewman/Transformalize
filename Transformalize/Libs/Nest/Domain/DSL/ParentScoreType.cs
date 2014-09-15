using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Domain.DSL
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum ParentScoreType
	{
		[EnumMember(Value = "none")]
		None = 0,
		[EnumMember(Value = "score")]
		Score
	}
}
