#region License
// /*
// See license included in this library folder.
// */
#endregion

using System.Data;
using Transformalize.Main.Providers;

namespace Transformalize.Libs.Rhino.Etl.Operations
{
    /// <summary>
    ///     Base class for operations that directly manipulate ADO.Net
    ///     It is important to remember that this is supposed to be a deep base class, not to be
    ///     directly inherited or used
    /// </summary>
    public abstract class AbstractCommandOperation : AbstractDatabaseOperation
    {
        /// <summary>
        ///     The current command
        /// </summary>
        protected IDbCommand currentCommand;

        protected AbstractCommandOperation(AbstractConnection connection)
            : base(connection)
        {
        }

        /// <summary>
        ///     Adds the parameter to the current command
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        protected void AddParameter(string name, object value)
        {
            AddParameter(currentCommand, name, value);
        }

        /// <summary>
        ///     Begins a transaction conditionally based on the UseTransaction property
        /// </summary>
        /// <param name="connection">The IDbConnection object you are working with</param>
        /// <returns>An open IDbTransaction object or null.</returns>
        protected IDbTransaction BeginTransaction(IDbConnection connection)
        {
            return UseTransaction ? connection.BeginTransaction() : null;
        }
    }
}