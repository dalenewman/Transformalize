using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum TermVectorOption
	{
		[EnumMember(Value = "no")]
		No,
		[EnumMember(Value = "yes")]
		Yes,
		[EnumMember(Value = "with_offsets")]
		WithOffsets,
		[EnumMember(Value = "with_positions")]
		WithPositions,
		[EnumMember(Value = "with_positions_offsets")]
		WithPositionsOffsets,
		[EnumMember(Value = "with_positions_offsets_payloads")]
        WithPositionsOffsetsPayloads
	}
}
