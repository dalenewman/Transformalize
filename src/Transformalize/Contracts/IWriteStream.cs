using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Contracts {
   /// <summary>Optional consumer of an asynchronous row stream.</summary>
   public interface IWriteStream {
      Task WriteStreamAsync(IAsyncEnumerable<IRow> rows, CancellationToken token = default);
   }
}
