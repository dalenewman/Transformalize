using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.Paths;

namespace Transformalize.Libs.Nest.Domain.DSL
{
	public interface IPathInfo<TParameters> 
		where TParameters : IRequestParameters, new()
	{
		ElasticsearchPathInfo<TParameters> ToPathInfo(IConnectionSettingsValues settings);
	}
}