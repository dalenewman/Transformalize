using System.IO;
using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Serialization;
using Transformalize.Libs.Nest.DSL;

namespace Transformalize.Libs.Nest.ExposedInternals
{

	//TODO It would be very nice if we can get rid of this interface
	public interface INestSerializer : IElasticsearchSerializer
	{
		string SerializeBulkDescriptor(IBulkRequest bulkRequest);

		string SerializeMultiSearch(IMultiSearchRequest multiSearchRequest);

		T DeserializeInternal<T>(Stream stream, JsonConverter converter);
	}
}
