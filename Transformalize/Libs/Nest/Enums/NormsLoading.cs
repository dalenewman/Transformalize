using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum NormsLoading
	{
		[EnumMember(Value = "lazy")]
		Lazy,
		[EnumMember(Value = "eager")]
		Eager
	}
}
