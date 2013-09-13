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
using System.Reflection;

namespace Transformalize.Libs.NLog.Targets
{
    /// <summary>
    ///     Calls the specified static method on each log message and passes contextual parameters to it.
    /// </summary>
    /// <seealso href="http://nlog-project.org/wiki/MethodCall_target">Documentation on NLog Wiki</seealso>
    /// <example>
    ///     <p>
    ///         To set up the target in the <a href="config.html">configuration file</a>,
    ///         use the following syntax:
    ///     </p>
    ///     <code lang="XML" source="examples/targets/Configuration File/MethodCall/NLog.config" />
    ///     <p>
    ///         This assumes just one target and a single rule. More configuration
    ///         options are described <a href="config.html">here</a>.
    ///     </p>
    ///     <p>
    ///         To set up the log target programmatically use code like this:
    ///     </p>
    ///     <code lang="C#" source="examples/targets/Configuration API/MethodCall/Simple/Example.cs" />
    /// </example>
    [Target("MethodCall")]
    public sealed class MethodCallTarget : MethodCallTargetBase
    {
        /// <summary>
        ///     Gets or sets the class name.
        /// </summary>
        /// <docgen category='Invocation Options' order='10' />
        public string ClassName { get; set; }

        /// <summary>
        ///     Gets or sets the method name. The method must be public and static.
        /// </summary>
        /// <docgen category='Invocation Options' order='10' />
        public string MethodName { get; set; }

        private MethodInfo Method { get; set; }

        /// <summary>
        ///     Initializes the target.
        /// </summary>
        protected override void InitializeTarget()
        {
            base.InitializeTarget();

            if (ClassName != null && MethodName != null)
            {
                var targetType = Type.GetType(ClassName);
                Method = targetType.GetMethod(MethodName);
            }
            else
            {
                Method = null;
            }
        }

        /// <summary>
        ///     Calls the specified Method.
        /// </summary>
        /// <param name="parameters">Method parameters.</param>
        protected override void DoInvoke(object[] parameters)
        {
            if (Method != null)
            {
                Method.Invoke(null, parameters);
            }
        }
    }
}