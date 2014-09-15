using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum IndexOptions
	{
		[EnumMember(Value="docs")]
		Docs,
		[EnumMember(Value = "freqs")]
		Freqs,
		[EnumMember(Value = "positions")]
		Positions
	}
}
