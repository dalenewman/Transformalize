using Transformalize.Libs.Elasticsearch.Net.Exceptions;

namespace Transformalize.Libs.Elasticsearch.Net.Domain.Response
{
	public class ElasticsearchServerError
	{
		public int Status { get; set; }
		public string Error { get; set; }
		public string ExceptionType { get; set; }

		internal static ElasticsearchServerError Create(OneToOneServerException e)
		{
			if (e == null) return null;
			return new ElasticsearchServerError
			{
				Status = e.status,
				Error = e.error
			};
		}
	}
}