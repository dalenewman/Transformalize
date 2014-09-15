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
		public IGetMappingResponse GetMapping<T>(Func<GetMappingDescriptor<T>, GetMappingDescriptor<T>> selector = null)
			where T : class
		{
			selector = selector ?? (s => s);
			return this.Dispatch<GetMappingDescriptor<T>, GetMappingRequestParameters, GetMappingResponse>(
				selector,
				(p, d) => this.RawDispatch.IndicesGetMappingDispatch<GetMappingResponse>(
					p.DeserializationState(new Func<IElasticsearchResponse, Stream, GetMappingResponse>((r, s) => DeserializeGetMappingResponse(r, d, s)))
				)
			);
		}

		/// <inheritdoc />
		public IGetMappingResponse GetMapping(IGetMappingRequest getMappingRequest)
		{
			return this.Dispatch<IGetMappingRequest, GetMappingRequestParameters, GetMappingResponse>(
				getMappingRequest,
				(p, d) => this.RawDispatch.IndicesGetMappingDispatch<GetMappingResponse>(
					p.DeserializationState(new Func<IElasticsearchResponse, Stream, GetMappingResponse>((r, s) => DeserializeGetMappingResponse(r, d, s)))
				)
			);
		}

		/// <inheritdoc />
		public Task<IGetMappingResponse> GetMappingAsync<T>(Func<GetMappingDescriptor<T>, GetMappingDescriptor<T>> selector = null)
			where T : class
		{
			selector = selector ?? (s => s);
			return this.DispatchAsync<GetMappingDescriptor<T>, GetMappingRequestParameters, GetMappingResponse, IGetMappingResponse>(
				selector,
				(p, d) => this.RawDispatch.IndicesGetMappingDispatchAsync<GetMappingResponse>(
					p.DeserializationState(new Func<IElasticsearchResponse, Stream, GetMappingResponse>((r, s) => DeserializeGetMappingResponse(r, d, s)))
				)
			);
		}

		/// <inheritdoc />
		public Task<IGetMappingResponse> GetMappingAsync(IGetMappingRequest getMappingRequest)
		{
			return this.DispatchAsync<IGetMappingRequest, GetMappingRequestParameters, GetMappingResponse, IGetMappingResponse>(
				getMappingRequest,
				(p, d) => this.RawDispatch.IndicesGetMappingDispatchAsync<GetMappingResponse>(
					p.DeserializationState(new Func<IElasticsearchResponse, Stream, GetMappingResponse>((r, s) => DeserializeGetMappingResponse(r, d, s)))
				)
			);
		}

		private GetMappingResponse DeserializeGetMappingResponse(IElasticsearchResponse response, IGetMappingRequest d, Stream stream)
		{
			var dict = response.Success
				? Serializer.Deserialize<GetRootObjectMappingWrapping>(stream)
				: null;
			return new GetMappingResponse(response, dict);
		}

	}
}