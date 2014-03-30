using System;

namespace Transformalize.Libs.Elasticsearch.Net.Exceptions
{
	public class OutOfNodesException : Exception
	{
		public OutOfNodesException(string message) : base(message)
		{
		}

		public OutOfNodesException(string message, Exception innerException) : base(message, innerException)
		{
			
		}
	}
}
