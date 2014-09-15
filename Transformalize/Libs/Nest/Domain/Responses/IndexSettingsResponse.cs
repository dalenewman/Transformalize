using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain.Settings;
using Transformalize.Libs.Nest.Extensions;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.Domain.Responses
{
	public interface IIndexSettingsResponse : IResponse
	{
		IndexSettings IndexSettings { get; }
	}

	[JsonObject(MemberSerialization.OptIn)]
	[JsonConverter(typeof(IndexSettingsResponseConverter))]
	public class IndexSettingsResponse : BaseResponse, IIndexSettingsResponse
	{
		[JsonIgnore]
		public IndexSettings IndexSettings
		{
			get { return Nodes.HasAny() ? Nodes.First().Value : null; }
		}

		[JsonIgnore]
		public Dictionary<string, IndexSettings> Nodes { get; set; }

	}
}
