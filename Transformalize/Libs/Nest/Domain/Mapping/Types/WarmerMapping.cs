using System.Collections.Generic;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.DSL;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Mapping.Types
{

	[JsonObject]
	[JsonConverter(typeof(WarmerMappingConverter))]
	public class WarmerMapping
	{

		public string Name { get; internal set; }

		public IEnumerable<TypeNameMarker> Types { get; internal set; }

		public ISearchRequest Source { get; internal set; }

	}
}