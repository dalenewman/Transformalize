#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Data;
using Transformalize.Libs.NLog;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Rhino.Etl.Infrastructure
{
    /// <summary>
    ///     Helper class to provide simple data access, when we want to access the ADO.Net
    ///     library directly.
    /// </summary>
    public static class Use
    {
        /// <summary>
        ///     Delegate to execute an action with a command
        ///     and return a result:
        ///     <typeparam name="T" />
        /// </summary>
        public delegate T Func<T>(IDbCommand command);

        /// <summary>
        ///     Delegate to execute an action with a command
        /// </summary>
        public delegate void Proc(IDbCommand command);

        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///     Gets or sets the active connection.
        /// </summary>
        /// <value>The active connection.</value>
        [ThreadStatic] private static IDbConnection ActiveConnection;

        /// <summary>
        ///     Gets or sets the active transaction.
        /// </summary>
        /// <value>The active transaction.</value>
        [ThreadStatic] private static IDbTransaction ActiveTransaction;

        /// <summary>
        ///     Gets or sets the transaction counter.
        /// </summary>
        /// <value>The transaction counter.</value>
        [ThreadStatic] private static int TransactionCounter;

        public static T Transaction<T>(AbstractConnection connection, Func<T> actionToExecute)
        {
            var result = default(T);
            Transaction(connection, delegate(IDbCommand command) { result = actionToExecute(command); });
            return result;
        }

        public static void Transaction(AbstractConnection connection, Proc actionToExecute)
        {
            Transaction(connection, IsolationLevel.Unspecified, actionToExecute);
        }

        public static void Transaction(AbstractConnection connection, IsolationLevel isolationLevel, Proc actionToExecute)
        {
            StartTransaction(connection, isolationLevel);
            try
            {
                using (var command = ActiveConnection.CreateCommand())
                {
                    command.Transaction = ActiveTransaction;
                    actionToExecute(command);
                }
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                DisposeTransaction();
            }
        }

        /// <summary>
        ///     Disposes the transaction.
        /// </summary>
        private static void DisposeTransaction()
        {
            if (TransactionCounter <= 0)
            {
                ActiveConnection.Dispose();
                ActiveConnection = null;
            }
        }

        /// <summary>
        ///     Rollbacks the transaction.
        /// </summary>
        private static void RollbackTransaction()
        {
            ActiveTransaction.Rollback();
            ActiveTransaction.Dispose();
            ActiveTransaction = null;
            TransactionCounter = 0;
        }

        /// <summary>
        ///     Commits the transaction.
        /// </summary>
        private static void CommitTransaction()
        {
            TransactionCounter--;
            if (TransactionCounter == 0 && ActiveTransaction != null)
            {
                ActiveTransaction.Commit();
                ActiveTransaction.Dispose();
                ActiveTransaction = null;
            }
        }

        private static void StartTransaction(AbstractConnection connection, IsolationLevel isolation)
        {
            if (TransactionCounter <= 0)
            {
                TransactionCounter = 0;
                ActiveConnection = Connection(connection);
                ActiveTransaction = ActiveConnection.BeginTransaction(isolation);
            }
            TransactionCounter++;
        }

        public static IDbConnection Connection(AbstractConnection connection)
        {
            var cn = connection.GetConnection();
            cn.Open();
            return cn;
        }
    }
}