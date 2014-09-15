using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Connection.Configuration;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.DSL;

namespace Transformalize.Libs.Nest.DSL.Common
{
	/// <summary>
	/// </summary>
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IRequest {}
	public interface IRequest<TParameters> : IPathInfo<TParameters>, IRequest
		where TParameters : IRequestParameters, new()
	{
		/// <summary>
		/// Used to describe request parameters not part of the body. e.q query string or 
		/// connection configuration overrides
		/// </summary>
		TParameters RequestParameters { get; set; }

		/// <summary>
		/// 
		/// </summary>
		IRequestConfiguration RequestConfiguration { get; set; }
	}
}