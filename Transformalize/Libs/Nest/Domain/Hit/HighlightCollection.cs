using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Hit
{

	[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
	public class HighlightFieldDictionary : Dictionary<string, Highlight>
	{
		public HighlightFieldDictionary(IDictionary<string, Highlight> dictionary = null)
		{
			if (dictionary == null)
				return;
			foreach(var kv in dictionary)
			{
				this.Add(kv.Key, kv.Value);
			}
		}
	}

	[JsonConverter(typeof(DictionaryKeysAreNotPropertyNamesJsonConverter))]
	public class HighlightDocumentDictionary : Dictionary<string, HighlightFieldDictionary>
	{

	}
}