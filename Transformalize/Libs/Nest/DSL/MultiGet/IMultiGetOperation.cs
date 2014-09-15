using System;
using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;

namespace Transformalize.Libs.Nest.DSL.MultiGet
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IMultiGetOperation
	{
		[JsonProperty(PropertyName = "_index")]
		IndexNameMarker Index { get; set; }
	
		[JsonProperty(PropertyName = "_type")]
		TypeNameMarker Type { get; set; }
		
		[JsonProperty(PropertyName = "_id")]
		string Id { get; set; }
		
		[JsonProperty(PropertyName = "fields")]
		IList<PropertyPathMarker> Fields { get; set; }
		
		[JsonProperty(PropertyName = "_routing")]
		string Routing { get; set; }
		
		Type ClrType { get; }
	}
}