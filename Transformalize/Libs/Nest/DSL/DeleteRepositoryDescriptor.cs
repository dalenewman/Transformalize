using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{

	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IDeleteRepositoryRequest : IRepositoryPath<DeleteRepositoryRequestParameters> { }

	internal static class DeleteRepositoryPathInfo
	{
		public static void Update(ElasticsearchPathInfo<DeleteRepositoryRequestParameters> pathInfo, IDeleteRepositoryRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.DELETE;
		}
	}
	
	public partial class DeleteRepositoryRequest : RepositoryPathBase<DeleteRepositoryRequestParameters>, IDeleteRepositoryRequest
	{
		public DeleteRepositoryRequest(string repositoryName) : base(repositoryName) { }

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<DeleteRepositoryRequestParameters> pathInfo)
		{
			DeleteRepositoryPathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("SnapshotDeleteRepository")]
	public partial class DeleteRepositoryDescriptor : RepositoryPathDescriptor<DeleteRepositoryDescriptor, DeleteRepositoryRequestParameters>, IDeleteRepositoryRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<DeleteRepositoryRequestParameters> pathInfo)
		{
			DeleteRepositoryPathInfo.Update(pathInfo, this);
		}

	}
}
