#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using Transformalize.Libs.Rhino.Etl.Infrastructure;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.Rhino.Etl.Pipelines;
using Transformalize.Main;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Rhino.Etl {
    /// <summary>
    ///     A single etl process
    /// </summary>
    public abstract class EtlProcess : EtlProcessBase<EtlProcess>, IDisposable {
        private IPipelineExecuter _pipelineExecuter = new ThreadPoolPipelineExecuter();

        protected EtlProcess(Process process) : base(process) {}

        /// <summary>
        ///     Gets the pipeline executer.
        /// </summary>
        /// <value>The pipeline executer.</value>
        public IPipelineExecuter PipelineExecuter {
            get { return _pipelineExecuter; }
            set {
                Debug("Setting PipelineExecutor to {0}", value.GetType().ToString());
                _pipelineExecuter = value;
            }
        }

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose() {
            foreach (var operation in Operations) {
                operation.Dispose();
            }
        }

        #endregion

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        protected abstract void Initialize();

        /// <summary>
        ///     Executes this process
        /// </summary>
        public void Execute() {
            Initialize();
            MergeLastOperationsToOperations();
            RegisterToOperationsEvents();
            Trace("Executing {0}", Name);
            PipelineExecuter.Execute(Name, Operations, TranslateRows);
            PostProcessing();
        }

        /// <summary>
        ///     Translate the rows from one representation to another
        /// </summary>
        public virtual IEnumerable<Row> TranslateRows(IEnumerable<Row> rows) {
            return rows;
        }

        private void RegisterToOperationsEvents() {
            foreach (var operation in Operations) {
                operation.OnRowProcessed += OnRowProcessed;
                operation.OnFinishedProcessing += OnFinishedProcessing;
            }
        }


        /// <summary>
        ///     Called when this process has finished processing.
        /// </summary>
        /// <param name="op">The op.</param>
        protected virtual void OnFinishedProcessing(IOperation op) {
            Trace("Finished {0}: {1}", op.Name, op.Statistics);
        }

        /// <summary>
        ///     Allow derived class to deal with custom logic after all the internal steps have been executed
        /// </summary>
        protected virtual void PostProcessing() {
        }

        /// <summary>
        ///     Called when a row is processed.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <param name="dictionary">The dictionary.</param>
        protected virtual void OnRowProcessed(IOperation op, Row dictionary) {
            if (op.Statistics.OutputtedRows % op.LogRows == 0) {
                TflLogger.Info(op.ProcessName, op.EntityName, "Processed {0} rows in {1}", op.Statistics.OutputtedRows, op.Name);
            }
        }

        protected static T ExecuteScalar<T>(AbstractConnection connection, string commandText) {
            return Use.Transaction(connection, delegate(IDbCommand cmd) {
                cmd.CommandText = commandText;
                var scalar = cmd.ExecuteScalar();
                return (T)(scalar ?? default(T));
            });
        }

        /// <summary>
        ///     Gets all errors that occured during the execution of this process
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Exception> GetAllErrors() {
            foreach (var error in Errors) {
                yield return error;
            }
            foreach (var error in _pipelineExecuter.GetAllErrors()) {
                yield return error;
            }
            foreach (var operation in Operations) {
                foreach (var exception in operation.GetAllErrors()) {
                    yield return exception;
                }
            }
        }
    }
}