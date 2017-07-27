using System.Collections.Generic;
using Transformalize.Configuration;

namespace Transformalize.Contracts {
    public interface IInputProvider {
        /// <summary>
        /// Get the maximum input version respecting the filter, or null if no version is defined
        /// </summary>
        object GetMaxVersion();

        Schema GetSchema(Entity entity = null);

        /// <summary>
        /// Read all or just what is necessary from the input
        /// </summary>
        /// <returns></returns>
        IEnumerable<IRow> Read();

    }
}
