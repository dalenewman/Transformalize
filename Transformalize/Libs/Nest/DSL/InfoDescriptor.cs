using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Common;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IInfoRequest : IRequest<InfoRequestParameters> { }

	internal static class InfoPathInfo
	{
		public static void Update(ElasticsearchPathInfo<InfoRequestParameters> pathInfo, IInfoRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.GET;
		}
	}
	
	public partial class InfoRequest : BasePathRequest<InfoRequestParameters>, IInfoRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<InfoRequestParameters> pathInfo)
		{
			InfoPathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("Info")]
	public partial class InfoDescriptor : BasePathDescriptor<InfoDescriptor, InfoRequestParameters>, IInfoRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<InfoRequestParameters> pathInfo)
		{
			InfoPathInfo.Update(pathInfo, this);
		}
	}
}
