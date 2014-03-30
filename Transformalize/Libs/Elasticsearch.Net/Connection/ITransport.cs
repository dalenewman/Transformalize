using System.Collections.Specialized;
using System.Threading.Tasks;
using Transformalize.Libs.Elasticsearch.Net.Domain;
using Transformalize.Libs.Elasticsearch.Net.Serialization;

namespace Transformalize.Libs.Elasticsearch.Net.Connection
{
	public interface ITransport
	{
		IConnectionConfigurationValues Settings { get; }
		IElasticsearchSerializer Serializer { get; }
		
		ElasticsearchResponse DoRequest(string method, string path, object data = null, NameValueCollection queryString = null, int retried = 0, int? seed = null);

		Task<ElasticsearchResponse> DoRequestAsync(
			string method, 
			string path, 
			object data = null, NameValueCollection queryString = null, int retried = 0, int? seed = null);
	}

	public interface ITransportValues
	{
		IElasticsearchSerializer Serializer { get; }
	}
}