using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Alias;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Mapping.Types
{
	//	[JsonConverter(typeof(TemplateMappingConverter))]
	//	[JsonObject(MemberSerialization.OptIn)]
	public class TemplateMapping
	{
		public TemplateMapping()
		{
		}

		[JsonProperty("template")]
		public string Template { get; set; }

		[JsonProperty("order")]
		public int? Order { get; set; }

		[JsonProperty("settings")]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		public FluentDictionary<string, object> Settings { get; set; }

		[JsonProperty("mappings")]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		public Dictionary<string, RootObjectMapping> Mappings { get; set; }

		[JsonProperty("warmers")]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		public Dictionary<string, WarmerMapping> Warmers { get; set; }
		
		[JsonProperty("aliases")]
		[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
		public Dictionary<string, ICreateAliasOperation> Aliases { get; set; }
	}
}