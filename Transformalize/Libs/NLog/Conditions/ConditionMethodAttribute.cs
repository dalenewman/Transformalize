#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using Transformalize.Libs.NLog.Config;

namespace Transformalize.Libs.NLog.Conditions
{
    /// <summary>
    ///     Marks class as a log event Condition and assigns a name to it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class ConditionMethodAttribute : NameBaseAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ConditionMethodAttribute" /> class.
        /// </summary>
        /// <param name="name">Condition method name.</param>
        public ConditionMethodAttribute(string name)
            : base(name)
        {
        }
    }
}