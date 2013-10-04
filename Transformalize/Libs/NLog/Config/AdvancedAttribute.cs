#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Marks the class or a member as advanced. Advanced classes and members are hidden by
    ///     default in generated documentation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AdvancedAttribute : Attribute
    {
    }
}