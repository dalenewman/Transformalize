using Transformalize.Libs.Elasticsearch.Net;

namespace Transformalize.Libs.Nest
{
	internal partial class RawDispatch 
	{
		protected IElasticsearchClient Raw { get; set; }

		public RawDispatch(IElasticsearchClient rawElasticClient)
		{
			this.Raw = rawElasticClient;
		}
	}
}
