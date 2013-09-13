using System;
using System.Collections.Generic;
using System.Data;
using Transformalize.Main.Providers;
using Transformalize.Libs.Rhino.Etl.Infrastructure;
using Transformalize.Libs.Rhino.Etl.Operations;
using Transformalize.Libs.Rhino.Etl.Pipelines;

namespace Transformalize.Libs.Rhino.Etl
{
    /// <summary>
    ///     A single etl process
    /// </summary>
    public abstract class EtlProcess : EtlProcessBase<EtlProcess>, IDisposable
    {
        private readonly string _name;
        private IPipelineExecuter _pipelineExecuter = new ThreadPoolPipelineExecuter();

        protected EtlProcess(string name)
        {
            _name = name;
        }

        /// <summary>
        ///     Gets the pipeline executer.
        /// </summary>
        /// <value>The pipeline executer.</value>
        public IPipelineExecuter PipelineExecuter
        {
            get { return _pipelineExecuter; }
            set
            {
                Info("Setting PipelineExecutor to {0}", value.GetType().ToString());
                _pipelineExecuter = value;
            }
        }


        /// <summary>
        ///     Gets a new partial process that we can work with
        /// </summary>
        protected static PartialProcessOperation Partial
        {
            get { return new PartialProcessOperation(); }
        }

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            foreach (IOperation operation in operations)
            {
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
        public void Execute()
        {
            Initialize();
            MergeLastOperationsToOperations();
            RegisterToOperationsEvents();
            Trace("Executing {0}", Name);
            PipelineExecuter.Execute(Name, operations, TranslateRows);
            PostProcessing();
        }

        /// <summary>
        ///     Translate the rows from one representation to another
        /// </summary>
        public virtual IEnumerable<Row> TranslateRows(IEnumerable<Row> rows)
        {
            return rows;
        }

        private void RegisterToOperationsEvents()
        {
            foreach (IOperation operation in operations)
            {
                operation.OnRowProcessed += OnRowProcessed;
                operation.OnFinishedProcessing += OnFinishedProcessing;
            }
        }


        /// <summary>
        ///     Called when this process has finished processing.
        /// </summary>
        /// <param name="op">The op.</param>
        protected virtual void OnFinishedProcessing(IOperation op)
        {
            Trace("Finished {0}: {1}", op.Name, op.Statistics);
        }

        /// <summary>
        ///     Allow derived class to deal with custom logic after all the internal steps have been executed
        /// </summary>
        protected virtual void PostProcessing()
        {
        }

        /// <summary>
        ///     Called when a row is processed.
        /// </summary>
        /// <param name="op">The operation.</param>
        /// <param name="dictionary">The dictionary.</param>
        protected virtual void OnRowProcessed(IOperation op, Row dictionary)
        {
            if (op.Statistics.OutputtedRows%10000 == 0)
                Info("Processed {0} rows in {1}", op.Statistics.OutputtedRows, op.Name);
            else
            {
                if (op.Statistics.OutputtedRows%1000 == 0)
                {
                    Debug("Processed {0} rows in {1}", op.Statistics.OutputtedRows, op.Name);
                }
                else
                {
                    if (op.Statistics.OutputtedRows%10 == 0)
                        Trace("Processed {0} rows in {1}", op.Statistics.OutputtedRows, op.Name);
                }
            }
        }

        protected static T ExecuteScalar<T>(AbstractConnection connection, string commandText)
        {
            return Use.Transaction(connection, delegate(IDbCommand cmd)
                                                   {
                                                       cmd.CommandText = commandText;
                                                       object scalar = cmd.ExecuteScalar();
                                                       return (T) (scalar ?? default(T));
                                                   });
        }

        /// <summary>
        ///     Gets all errors that occured during the execution of this process
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Exception> GetAllErrors()
        {
            foreach (Exception error in Errors)
            {
                yield return error;
            }
            foreach (Exception error in _pipelineExecuter.GetAllErrors())
            {
                yield return error;
            }
            foreach (IOperation operation in operations)
            {
                foreach (Exception exception in operation.GetAllErrors())
                {
                    yield return exception;
                }
            }
        }
    }
}