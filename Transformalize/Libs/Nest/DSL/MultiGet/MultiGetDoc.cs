using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;

namespace Transformalize.Libs.Nest.DSL.MultiGet
{
	internal class MultiGetDoc
	{
		[JsonProperty(PropertyName = "_index")]
		internal string Index { get; set; }
		
		[JsonProperty(PropertyName = "_type")]
		internal string Type { get; set; }
		
		[JsonProperty(PropertyName = "_id")]
		internal string Id { get; set; }
		
		[JsonProperty(PropertyName = "fields")]
		internal IEnumerable<PropertyPathMarker> Fields { get; set; }
		
		[JsonProperty(PropertyName = "_routing")]
		internal string Routing { get; set; }
		
	}
}