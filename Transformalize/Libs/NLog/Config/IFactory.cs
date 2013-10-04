#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Reflection;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Provides means to populate factories of named items (such as targets, layouts, layout renderers, etc.).
    /// </summary>
    internal interface IFactory
    {
        void Clear();

        void ScanAssembly(Assembly theAssembly, string prefix);

        void RegisterType(Type type, string itemNamePrefix);
    }
}