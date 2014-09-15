using System;

namespace Transformalize.Libs.Elasticsearch.Net.Connection.RequestState
{
	public interface IRequestTimings : IDisposable
	{
		void Finish(bool success, int? httpStatusCode);
	}
}