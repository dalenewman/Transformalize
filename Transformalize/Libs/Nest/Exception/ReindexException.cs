using Transformalize.Libs.Elasticsearch.Net.Domain.Response;

namespace Transformalize.Libs.Nest.Exception
{
	public class ReindexException: System.Exception
	{
		public IElasticsearchResponse Status { get; private set; }

		public ReindexException(IElasticsearchResponse status, string message = null) : base(message)
		{
			this.Status = status;
		}
	}
}