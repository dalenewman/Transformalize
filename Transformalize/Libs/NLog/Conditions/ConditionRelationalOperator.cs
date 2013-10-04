#region License
// /*
// See license included in this library folder.
// */
#endregion
namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Relational operators used in conditions.
    /// </summary>
    internal enum ConditionRelationalOperator
    {
        /// <summary>
        ///     Equality (==).
        /// </summary>
        Equal,

        /// <summary>
        ///     Inequality (!=).
        /// </summary>
        NotEqual,

        /// <summary>
        ///     Less than (&lt;).
        /// </summary>
        Less,

        /// <summary>
        ///     Greater than (&gt;).
        /// </summary>
        Greater,

        /// <summary>
        ///     Less than or equal (&lt;=).
        /// </summary>
        LessOrEqual,

        /// <summary>
        ///     Greater than or equal (&gt;=).
        /// </summary>
        GreaterOrEqual,
    }
}