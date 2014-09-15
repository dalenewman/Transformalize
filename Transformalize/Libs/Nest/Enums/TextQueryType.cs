using System.Runtime.Serialization;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Newtonsoft.Json.Converters;

namespace Transformalize.Libs.Nest.Enums
{
	[JsonConverter(typeof(StringEnumConverter))]
    public enum TextQueryType
    {
		[EnumMember(Value = "best_fields")]
        BestFields,
		[EnumMember(Value = "most_fields")]
        MostFields,
		[EnumMember(Value = "cross_fields")]
        CrossFields,
		[EnumMember(Value = "phrase")]
        Phrase,
		[EnumMember(Value = "phrase_prefix")]
        PhrasePrefix
    }
}