#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Attribute used to mark the required parameters for targets,
    ///     layout targets and filters.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RequiredParameterAttribute : Attribute
    {
    }
}