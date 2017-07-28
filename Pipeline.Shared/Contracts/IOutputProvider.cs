using System;
using System.Collections.Generic;

namespace Transformalize.Contracts {

    //TODO: remove IInputVersionDetector, IVersionDetector, 

    public interface IOutputProvider : IDisposable {
        /// <summary>
        /// Initialize the output:
        /// * destroy existing structures
        /// * create existing structures 
        /// </summary>
        void Initialize();

        /// <summary>
        /// Get the maximum output version that is not marked as deleted, or null if no version defined
        /// </summary>
        /// <returns></returns>
        object GetMaxVersion();

        /// <summary>
        /// Get the maximum TflBatchId in the output, or null if init mode
        /// </summary>
        /// <returns></returns>
        int GetNextTflBatchId();

        /// <summary>
        /// Get the maximum TflKey in the output, or null if init mode
        /// </summary>
        /// <returns></returns>
        int GetMaxTflKey();

        /// <summary>
        /// provider specific start actions
        /// </summary>
        void Start();

        // provider specific end actions
        void End();

        /// <summary>
        /// write all or just what is necessary to the output
        /// determine inserts vs. updates
        /// </summary>
        /// <param name="rows"></param>
        void Write(IEnumerable<IRow> rows);

        /// <summary>
        /// When delete is enabled, you must determine what needs to be deleted, and then MARK them as deleted, TflDeleted = true
        /// </summary>
        void Delete();

        // Read all primary key, TflHashCode, and TflDeleted (support for delete)
        IEnumerable<IRow> ReadKeys();

        /// <summary>
        /// Given the input (or most likely a batch of input)
        /// Find the matching primary keys, along with TflDeleted, and TflHashCode (system fields every output has)
        /// TODO: make ITakeAndReturnRows, and IReadInputKeysAndHashcodes obsolete
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        IEnumerable<IRow> Match(IEnumerable<IRow> rows);

    }
}
