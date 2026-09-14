using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Transformalize.Contracts {
   /// <summary>Optional sequence operation. Preserve state and ordering for the entire enumeration.</summary>
   public interface IOperateStream {
      IAsyncEnumerable<IRow> OperateStreamAsync(IAsyncEnumerable<IRow> rows, CancellationToken token = default);
   }
}
