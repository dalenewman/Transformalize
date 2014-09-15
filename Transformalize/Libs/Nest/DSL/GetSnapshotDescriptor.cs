using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IGetSnapshotRequest : IRepositorySnapshotPath<GetSnapshotRequestParameters> { }

	internal static class GetSnapshotPathInfo
	{
		public static void Update(ElasticsearchPathInfo<GetSnapshotRequestParameters> pathInfo, IGetSnapshotRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.GET;
		}
	}
	
	public partial class GetSnapshotRequest : RepositorySnapshotPathBase<GetSnapshotRequestParameters>, IGetSnapshotRequest
	{
		public GetSnapshotRequest(string repository, string snapshot) : base(repository, snapshot) { }

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<GetSnapshotRequestParameters> pathInfo)
		{
			GetSnapshotPathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("SnapshotGet")]
	public partial class GetSnapshotDescriptor : RepositorySnapshotPathDescriptor<GetSnapshotDescriptor, GetSnapshotRequestParameters>, IGetSnapshotRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<GetSnapshotRequestParameters> pathInfo)
		{
			GetSnapshotPathInfo.Update(pathInfo, this);
		}

	}
}
