using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;

namespace Transformalize.Libs.Nest.DSL.Common
{
	public abstract class BasePathRequest<TParameters> : BaseRequest<TParameters>
		where TParameters : IRequestParameters, new()
	{
		
		//[JsonIgnore]
		//public IRequestConfiguration RequestConfiguration
		//{	
		//	get { return base._requestConfiguration; }
		//	set { base._requestConfiguration = value; }
		//}
	}
}