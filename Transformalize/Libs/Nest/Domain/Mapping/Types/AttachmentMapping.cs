using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Mapping.Types
{
	[JsonObject(MemberSerialization.OptIn)]
	public class AttachmentMapping : IElasticType
	{
		public PropertyNameMarker Name { get; set; }

		[JsonProperty("type")]
		public virtual TypeNameMarker Type { get { return new TypeNameMarker { Name = "attachment" }; } }

		[JsonProperty("similarity")]
		public string Similarity { get; set; }

		[JsonProperty("fields"), JsonConverter(typeof(ElasticCoreTypeConverter))]
		public IDictionary<PropertyNameMarker, IElasticCoreType> Fields { get; set; }

		public AttachmentMapping()
		{
			this.Fields = new Dictionary<PropertyNameMarker, IElasticCoreType>();
		}

	}
}