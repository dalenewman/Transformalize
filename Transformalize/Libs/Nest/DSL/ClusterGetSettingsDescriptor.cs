using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Common;

namespace Transformalize.Libs.Nest.DSL
{

	internal static class ClusterGetSettingsPathInfo
	{
		public static void Update(ElasticsearchPathInfo<ClusterGetSettingsRequestParameters> pathInfo, IClusterGetSettingsRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.GET;
		}
	}

	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IClusterGetSettingsRequest : IRequest<ClusterGetSettingsRequestParameters>
	{

	}
	
	public partial class ClusterGetSettingsRequest : BaseRequest<ClusterGetSettingsRequestParameters>, IClusterGetSettingsRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<ClusterGetSettingsRequestParameters> pathInfo)
		{
			ClusterGetSettingsPathInfo.Update(pathInfo, this);
		}
	}

	public partial class ClusterGetSettingsDescriptor : BasePathDescriptor<ClusterGetSettingsDescriptor, ClusterGetSettingsRequestParameters>
		, IClusterGetSettingsRequest
	{
		protected IClusterGetSettingsRequest Self { get { return this; } }

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<ClusterGetSettingsRequestParameters> pathInfo)
		{
			ClusterGetSettingsPathInfo.Update(pathInfo, this);
		}
	}
}
