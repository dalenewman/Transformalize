using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IDeleteSnapshotRequest : IRepositorySnapshotPath<DeleteSnapshotRequestParameters> { }

	internal static class DeleteSnapshotPathInfo
	{
		public static void Update(ElasticsearchPathInfo<DeleteSnapshotRequestParameters> pathInfo, IDeleteSnapshotRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.DELETE;
		}
	}
	
	public partial class DeleteSnapshotRequest : RepositorySnapshotPathBase<DeleteSnapshotRequestParameters>, IDeleteSnapshotRequest
	{
		public DeleteSnapshotRequest(string repository, string snapshot) : base(repository, snapshot) { }

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<DeleteSnapshotRequestParameters> pathInfo)
		{
			DeleteSnapshotPathInfo.Update(pathInfo, this);
		}
	}
	[DescriptorFor("SnapshotDelete")]
	public partial class DeleteSnapshotDescriptor : RepositorySnapshotPathDescriptor<DeleteSnapshotDescriptor, DeleteSnapshotRequestParameters>, IDeleteSnapshotRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<DeleteSnapshotRequestParameters> pathInfo)
		{
			DeleteSnapshotPathInfo.Update(pathInfo, this);
		}

	}
}
