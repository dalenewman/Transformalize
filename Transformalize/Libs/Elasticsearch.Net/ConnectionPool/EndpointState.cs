using System;

namespace Transformalize.Libs.Elasticsearch.Net.ConnectionPool
{
	public class EndpointState
	{
		public int _attempts = -1;
		public DateTime date = new DateTime();
	}
}
