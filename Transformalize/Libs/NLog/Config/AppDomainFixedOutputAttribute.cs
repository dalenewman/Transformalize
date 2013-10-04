#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Identifies that the output of layout or layout render does not change for the lifetime of the current appdomain.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AppDomainFixedOutputAttribute : Attribute
    {
    }
}