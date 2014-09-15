using System;
using System.Threading.Tasks;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Responses;
using Transformalize.Libs.Nest.DSL;

namespace Transformalize.Libs.Nest
{
	public partial class ElasticClient
	{
		/// <inheritdoc />
		public IShardsOperationResponse ClearCache(Func<ClearCacheDescriptor, ClearCacheDescriptor> selector = null)
		{
			selector = selector ?? (s => s);
			return this.Dispatch<ClearCacheDescriptor, ClearCacheRequestParameters, ShardsOperationResponse>(
				selector,
				(p, d) => this.RawDispatch.IndicesClearCacheDispatch<ShardsOperationResponse>(p)
			);
		}

		/// <inheritdoc />
		public IShardsOperationResponse ClearCache(IClearCacheRequest clearCacheRequest)
		{
			return this.Dispatch<IClearCacheRequest, ClearCacheRequestParameters, ShardsOperationResponse>(
				clearCacheRequest,
				(p, d) => this.RawDispatch.IndicesClearCacheDispatch<ShardsOperationResponse>(p)
			);
		}

		/// <inheritdoc />
		public Task<IShardsOperationResponse> ClearCacheAsync(Func<ClearCacheDescriptor, ClearCacheDescriptor> selector = null)
		{
			selector = selector ?? (s => s);
			return this.DispatchAsync<ClearCacheDescriptor, ClearCacheRequestParameters, ShardsOperationResponse, IShardsOperationResponse>(
				selector,
				(p, d) => this.RawDispatch.IndicesClearCacheDispatchAsync<ShardsOperationResponse>(p)
			);
		}

		/// <inheritdoc />
		public Task<IShardsOperationResponse> ClearCacheAsync(IClearCacheRequest clearCacheRequest)
		{
			return this.DispatchAsync<IClearCacheRequest, ClearCacheRequestParameters, ShardsOperationResponse, IShardsOperationResponse>(
				clearCacheRequest,
				(p, d) => this.RawDispatch.IndicesClearCacheDispatchAsync<ShardsOperationResponse>(p)
			);
		}

	}
}