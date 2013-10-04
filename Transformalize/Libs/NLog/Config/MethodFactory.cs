#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Internal;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Factory for locating methods.
    /// </summary>
    /// <typeparam name="TClassAttributeType">The type of the class marker attribute.</typeparam>
    /// <typeparam name="TMethodAttributeType">The type of the method marker attribute.</typeparam>
    internal class MethodFactory<TClassAttributeType, TMethodAttributeType> : INamedItemFactory<MethodInfo, MethodInfo>, IFactory
        where TClassAttributeType : Attribute
        where TMethodAttributeType : NameBaseAttribute
    {
        private readonly Dictionary<string, MethodInfo> nameToMethodInfo = new Dictionary<string, MethodInfo>();

        /// <summary>
        ///     Gets a collection of all registered items in the factory.
        /// </summary>
        /// <returns>
        ///     Sequence of key/value pairs where each key represents the name
        ///     of the item and value is the <see cref="MethodInfo" /> of
        ///     the item.
        /// </returns>
        public IDictionary<string, MethodInfo> AllRegisteredItems
        {
            get { return nameToMethodInfo; }
        }

        /// <summary>
        ///     Scans the assembly for classes marked with <typeparamref name="TClassAttributeType" />
        ///     and methods marked with <typeparamref name="TMethodAttributeType" /> and adds them
        ///     to the factory.
        /// </summary>
        /// <param name="theAssembly">The assembly.</param>
        /// <param name="prefix">The prefix to use for names.</param>
        public void ScanAssembly(Assembly theAssembly, string prefix)
        {
            try
            {
                InternalLogger.Debug("ScanAssembly('{0}','{1}','{2}')", theAssembly.FullName, typeof (TClassAttributeType), typeof (TMethodAttributeType));
                foreach (var t in theAssembly.SafeGetTypes())
                {
                    RegisterType(t, prefix);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("Failed to add targets from '" + theAssembly.FullName + "': {0}", exception);
            }
        }

        /// <summary>
        ///     Registers the type.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <param name="itemNamePrefix">The item name prefix.</param>
        public void RegisterType(Type type, string itemNamePrefix)
        {
            if (type.IsDefined(typeof (TClassAttributeType), false))
            {
                foreach (var mi in type.GetMethods())
                {
                    var methodAttributes = (TMethodAttributeType[]) mi.GetCustomAttributes(typeof (TMethodAttributeType), false);
                    foreach (var attr in methodAttributes)
                    {
                        RegisterDefinition(itemNamePrefix + attr.Name, mi);
                    }
                }
            }
        }

        /// <summary>
        ///     Clears contents of the factory.
        /// </summary>
        public void Clear()
        {
            nameToMethodInfo.Clear();
        }

        /// <summary>
        ///     Registers the definition of a single method.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <param name="methodInfo">The method info.</param>
        public void RegisterDefinition(string name, MethodInfo methodInfo)
        {
            nameToMethodInfo[name] = methodInfo;
        }

        /// <summary>
        ///     Tries to retrieve method by name.
        /// </summary>
        /// <param name="name">The method name.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        ///     A value of <c>true</c> if the method was found, <c>false</c> otherwise.
        /// </returns>
        public bool TryCreateInstance(string name, out MethodInfo result)
        {
            return nameToMethodInfo.TryGetValue(name, out result);
        }

        /// <summary>
        ///     Retrieves method by name.
        /// </summary>
        /// <param name="name">Method name.</param>
        /// <returns>MethodInfo object.</returns>
        public MethodInfo CreateInstance(string name)
        {
            MethodInfo result;

            if (TryCreateInstance(name, out result))
            {
                return result;
            }

            throw new NLogConfigurationException("Unknown function: '" + name + "'");
        }

        /// <summary>
        ///     Tries to get method definition.
        /// </summary>
        /// <param name="name">The method .</param>
        /// <param name="result">The result.</param>
        /// <returns>
        ///     A value of <c>true</c> if the method was found, <c>false</c> otherwise.
        /// </returns>
        public bool TryGetDefinition(string name, out MethodInfo result)
        {
            return nameToMethodInfo.TryGetValue(name, out result);
        }
    }
}