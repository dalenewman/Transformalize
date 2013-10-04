#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Data.SqlClient;
using System.Reflection;

namespace Transformalize.Libs.Rhino.Etl.Infrastructure
{
    /// <summary>
    ///     Expose the batch functionality in ADO.Net 2.0
    ///     Microsoft in its wisdom decided to make my life hard and mark it internal.
    ///     Through the use of Reflection and some delegates magic, I opened up the functionality.
    ///     There is NO documentation for this, and likely zero support.
    ///     Use at your own risk, etc...
    ///     Observable performance benefits are 50%+ when used, so it is really worth it.
    /// </summary>
    public class SqlCommandSet : IDisposable
    {
        private static readonly Type sqlCmdSetType;
        private readonly PropGetter<SqlCommand> commandGetter;
        private readonly PropGetter<SqlConnection> connectionGetter;
        private readonly PropSetter<SqlConnection> connectionSetter;
        private readonly AppendCommand doAppend;
        private readonly DisposeCommand doDispose;
        private readonly ExecuteNonQueryCommand doExecuteNonQuery;
        private readonly object instance;
        private readonly PropSetter<int> timeoutSetter;
        private readonly PropSetter<SqlTransaction> transactionSetter;
        private int countOfCommands;

        static SqlCommandSet()
        {
            var sysData = Assembly.Load("System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            sqlCmdSetType = sysData.GetType("System.Data.SqlClient.SqlCommandSet");
            Guard.Against(sqlCmdSetType == null, "Could not find SqlCommandSet!");
        }

        /// <summary>
        ///     Creates a new instance of SqlCommandSet
        /// </summary>
        public SqlCommandSet()
        {
            instance = Activator.CreateInstance(sqlCmdSetType, true);

            timeoutSetter = (PropSetter<int>) Delegate.CreateDelegate(typeof (PropSetter<int>), instance, "set_CommandTimeout");
            connectionSetter = (PropSetter<SqlConnection>) Delegate.CreateDelegate(typeof (PropSetter<SqlConnection>), instance, "set_Connection");
            transactionSetter = (PropSetter<SqlTransaction>) Delegate.CreateDelegate(typeof (PropSetter<SqlTransaction>), instance, "set_Transaction");
            connectionGetter = (PropGetter<SqlConnection>) Delegate.CreateDelegate(typeof (PropGetter<SqlConnection>), instance, "get_Connection");
            commandGetter = (PropGetter<SqlCommand>) Delegate.CreateDelegate(typeof (PropGetter<SqlCommand>), instance, "get_BatchCommand");
            doAppend = (AppendCommand) Delegate.CreateDelegate(typeof (AppendCommand), instance, "Append");
            doExecuteNonQuery = (ExecuteNonQueryCommand) Delegate.CreateDelegate(typeof (ExecuteNonQueryCommand), instance, "ExecuteNonQuery");
            doDispose = (DisposeCommand) Delegate.CreateDelegate(typeof (DisposeCommand), instance, "Dispose");
        }

        /// <summary>
        ///     Return the batch command to be executed
        /// </summary>
        public SqlCommand BatchCommand
        {
            get { return commandGetter(); }
        }

        /// <summary>
        ///     The number of commands batched in this instance
        /// </summary>
        public int CountOfCommands
        {
            get { return countOfCommands; }
        }

        /// <summary>
        ///     The connection the batch will use
        /// </summary>
        public SqlConnection Connection
        {
            get { return connectionGetter(); }
            set { connectionSetter(value); }
        }

        /// <summary>
        ///     Set the timeout of the commandSet
        /// </summary>
        public int CommandTimeout
        {
            set { timeoutSetter(value); }
        }

        /// <summary>
        ///     The transaction the batch will run as part of
        /// </summary>
        public SqlTransaction Transaction
        {
            set { transactionSetter(value); }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            doDispose();
        }

        #region Delegate Definations

        private delegate void AppendCommand(SqlCommand command);

        private delegate void DisposeCommand();

        private delegate int ExecuteNonQueryCommand();

        private delegate T PropGetter<T>();

        private delegate void PropSetter<T>(T item);

        #endregion

        /// <summary>
        ///     Append a command to the batch
        /// </summary>
        /// <param name="command"></param>
        public void Append(SqlCommand command)
        {
            AssertHasParameters(command);
            doAppend(command);
            countOfCommands++;
        }

        /// <summary>
        ///     This is required because SqlClient.SqlCommandSet will throw if
        ///     the command has no parameters.
        /// </summary>
        /// <param name="command"></param>
        private static void AssertHasParameters(SqlCommand command)
        {
            if (command.Parameters.Count == 0)
            {
                throw new ArgumentException("A command in SqlCommandSet must have parameters. You can't pass hardcoded sql strings.");
            }
        }

        /// <summary>
        ///     Executes the batch
        /// </summary>
        /// <returns>
        ///     This seems to be returning the total number of affected rows in all queries
        /// </returns>
        public int ExecuteNonQuery()
        {
            Guard.Against<ArgumentException>(Connection == null, "Connection was not set! You must set the connection property before calling ExecuteNonQuery()");
            if (CountOfCommands == 0)
                return 0;
            return doExecuteNonQuery();
        }
    }
}