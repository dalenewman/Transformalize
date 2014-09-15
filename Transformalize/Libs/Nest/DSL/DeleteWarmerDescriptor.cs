using Transformalize.Libs.Newtonsoft.Json;
using Transformalize.Libs.Elasticsearch.Net.Domain.RequestParameters;
using Transformalize.Libs.Nest.Domain.Connection;
using Transformalize.Libs.Nest.Domain.DSL;
using Transformalize.Libs.Nest.Domain.Paths;
using Transformalize.Libs.Nest.DSL.Paths;

namespace Transformalize.Libs.Nest.DSL
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface IDeleteWarmerRequest : IIndicesOptionalTypesNamePath<DeleteWarmerRequestParameters> { }

	internal static class DeleteWarmerPathInfo
	{
		public static void Update(ElasticsearchPathInfo<DeleteWarmerRequestParameters> pathInfo, IDeleteWarmerRequest request)
		{
			pathInfo.HttpMethod = PathInfoHttpMethod.DELETE;
		}
	}
	
	public partial class DeleteWarmerRequest : IndicesOptionalTypesNamePathBase<DeleteWarmerRequestParameters>, IDeleteWarmerRequest
	{
		public DeleteWarmerRequest(string name)
		{
			this.Name = name;
		}

		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<DeleteWarmerRequestParameters> pathInfo)
		{
			DeleteWarmerPathInfo.Update(pathInfo, this);
		}
	}

	[DescriptorFor("IndicesDeleteWarmer")]
	public partial class DeleteWarmerDescriptor : IndicesOptionalTypesNamePathDescriptor<DeleteWarmerDescriptor, DeleteWarmerRequestParameters>, IDeleteWarmerRequest
	{
		protected override void UpdatePathInfo(IConnectionSettingsValues settings, ElasticsearchPathInfo<DeleteWarmerRequestParameters> pathInfo)
		{
			DeleteWarmerPathInfo.Update(pathInfo, this);
		}
	}
}
