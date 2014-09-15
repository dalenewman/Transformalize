using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Marker;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IDeleteMappingRequest : IIndexTypePath<DeleteMappingRequestParameters> { }
	public interface IDeleteMappingRequest<T> : IDeleteMappingRequest where T : class { }

	internal static class DeleteMappingPathInfo
	{
		public static void Update(ElasticsearchPathInfo<DeleteMappingRequestParameters> pathInfo, IDeleteMappingRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.DELETE;
		}
	}
	
	public partial class DeleteMappingRequest : IndexTypePathBase<DeleteMappingRequestParameters>, IDeleteMappingRequest
	{
		public DeleteMappingRequest(IndexNameMarker index, TypeNameMarker typeNameMarker) : base(index, typeNameMarker)
		{
		}

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<DeleteMappingRequestParameters> pathInfo)
		{
			DeleteMappingPathInfo.Update(pathInfo, this);
		}
	}
	public partial class DeleteMappingRequest<T> : IndexTypePathBase<DeleteMappingRequestParameters, T>, IDeleteMappingRequest
		where T : class
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<DeleteMappingRequestParameters> pathInfo)
		{
			DeleteMappingPathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("IndicesDeleteMapping")]
	public partial class DeleteMappingDescriptor<T> : IndexTypePathDescriptor<DeleteMappingDescriptor<T>, DeleteMappingRequestParameters, T>, IDeleteMappingRequest
		where T : class
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<DeleteMappingRequestParameters> pathInfo)
		{
			DeleteMappingPathInfo.Update(pathInfo, this);
		}
	}
}
