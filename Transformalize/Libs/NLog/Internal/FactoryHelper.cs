#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;

namespace Transformalize.Libs.NLog.Internal
{
    /// <summary>
    ///     Object construction helper.
    /// </summary>
    internal class FactoryHelper
    {
        private static readonly Type[] emptyTypes = new Type[0];
        private static readonly object[] emptyParams = new object[0];

        private FactoryHelper()
        {
        }

        internal static object CreateInstance(Type t)
        {
            var constructor = t.GetConstructor(emptyTypes);
            if (constructor != null)
            {
                return constructor.Invoke(emptyParams);
            }
            else
            {
                throw new NLogConfigurationException("Cannot access the constructor of type: " + t.FullName + ". Is the required permission granted?");
            }
        }
    }
}