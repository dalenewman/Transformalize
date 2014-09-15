using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Nest.Domain;
using Transformalize.Libs.Nest.DSL.Query;
using Transformalize.Libs.Nest.Extensions;
using Transformalize.Libs.Nest.Resolvers.Converters;

namespace Transformalize.Libs.Nest.DSL.Filter
{
	[JsonConverter(typeof(CustomJsonConverter))]
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class RawFilter : IQuery, ICustomJson
	{
		internal object Json { get; set; }
		bool IQuery.IsConditionless { get { return this.Json == null || this.Json.ToString().IsNullOrEmpty(); } }

		object ICustomJson.GetCustomJson()
		{
			return this.Json;
		}
	}
}
