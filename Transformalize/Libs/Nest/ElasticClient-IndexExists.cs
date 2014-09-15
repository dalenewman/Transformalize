using System;
using System.IO;
using System.Threading.Tasks;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Elasticsearch.Net.Domain.Response;
using Transformalize.Libs.Nest.Domain.Responses;
using Transformalize.Libs.Nest.DSL;

namespace Transformalize.Libs.Nest
{
    public partial class ElasticClient
	{

		/// <inheritdoc />
		public IExistsResponse IndexExists(Func<IndexExistsDescriptor, IndexExistsDescriptor> selector)
		{
			return this.Dispatch<IndexExistsDescriptor, IndexExistsRequestParameters, ExistsResponse>(
				selector,
				(p, d) => this.RawDispatch.IndicesExistsDispatch<ExistsResponse>(
					p.DeserializationState(new Func<IElasticsearchResponse, Stream, ExistsResponse>(DeserializeExistsResponse))
				)
			);
		}

		/// <inheritdoc />
		public IExistsResponse IndexExists(IIndexExistsRequest indexRequest)
		{
			return this.Dispatch<IIndexExistsRequest, IndexExistsRequestParameters, ExistsResponse>(
				indexRequest,
				(p, d) => this.RawDispatch.IndicesExistsDispatch<ExistsResponse>(
					p.DeserializationState(new Func<IElasticsearchResponse, Stream, ExistsResponse>(DeserializeExistsResponse))
				)
			);
		}

		/// <inheritdoc />
		public Task<IExistsResponse> IndexExistsAsync(Func<IndexExistsDescriptor, IndexExistsDescriptor> selector)
		{
			return this.DispatchAsync<IndexExistsDescriptor, IndexExistsRequestParameters, ExistsResponse, IExistsResponse>(
				selector,
				(p, d) => this.RawDispatch.IndicesExistsDispatchAsync<ExistsResponse>(
					p.DeserializationState(new Func<IElasticsearchResponse, Stream, ExistsResponse>(DeserializeExistsResponse))
				)
			);
		}

		/// <inheritdoc />
		public Task<IExistsResponse> IndexExistsAsync(IIndexExistsRequest indexRequest)
		{
			return this.DispatchAsync<IIndexExistsRequest, IndexExistsRequestParameters, ExistsResponse, IExistsResponse>(
				indexRequest,
				(p, d) => this.RawDispatch.IndicesExistsDispatchAsync<ExistsResponse>(
					p.DeserializationState(new Func<IElasticsearchResponse, Stream, ExistsResponse>(DeserializeExistsResponse))
				)
			);
		}


		private ExistsResponse DeserializeExistsResponse(IElasticsearchResponse response, Stream stream)
		{
			return new ExistsResponse(response);
		}
	}
}