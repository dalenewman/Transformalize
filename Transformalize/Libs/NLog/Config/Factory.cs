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
    ///     Factory for class-based items.
    /// </summary>
    /// <typeparam name="TBaseType">The base type of each item.</typeparam>
    /// <typeparam name="TAttributeType">The type of the attribute used to annotate itemss.</typeparam>
    internal class Factory<TBaseType, TAttributeType> : INamedItemFactory<TBaseType, Type>, IFactory
        where TBaseType : class
        where TAttributeType : NameBaseAttribute
    {
        private readonly Dictionary<string, GetTypeDelegate> items = new Dictionary<string, GetTypeDelegate>(StringComparer.OrdinalIgnoreCase);
        private readonly ConfigurationItemFactory parentFactory;

        internal Factory(ConfigurationItemFactory parentFactory)
        {
            this.parentFactory = parentFactory;
        }

        /// <summary>
        ///     Scans the assembly.
        /// </summary>
        /// <param name="theAssembly">The assembly.</param>
        /// <param name="prefix">The prefix.</param>
        public void ScanAssembly(Assembly theAssembly, string prefix)
        {
            try
            {
                InternalLogger.Debug("ScanAssembly('{0}','{1}','{2}')", theAssembly.FullName, typeof (TAttributeType), typeof (TBaseType));
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
            var attributes = (TAttributeType[]) type.GetCustomAttributes(typeof (TAttributeType), false);
            if (attributes != null)
            {
                foreach (var attr in attributes)
                {
                    RegisterDefinition(itemNamePrefix + attr.Name, type);
                }
            }
        }

        /// <summary>
        ///     Clears the contents of the factory.
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }

        /// <summary>
        ///     Registers a single type definition.
        /// </summary>
        /// <param name="name">The item name.</param>
        /// <param name="type">The type of the item.</param>
        public void RegisterDefinition(string name, Type type)
        {
            items[name] = () => type;
        }

        /// <summary>
        ///     Tries to get registed item definition.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="result">Reference to a variable which will store the item definition.</param>
        /// <returns>Item definition.</returns>
        public bool TryGetDefinition(string itemName, out Type result)
        {
            GetTypeDelegate del;

            if (!items.TryGetValue(itemName, out del))
            {
                result = null;
                return false;
            }

            try
            {
                result = del();
                return result != null;
            }
            catch (Exception ex)
            {
                if (ex.MustBeRethrown())
                {
                    throw;
                }

                // delegate invocation failed - type is not available
                result = null;
                return false;
            }
        }

        /// <summary>
        ///     Tries to create an item instance.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="result">The result.</param>
        /// <returns>True if instance was created successfully, false otherwise.</returns>
        public bool TryCreateInstance(string itemName, out TBaseType result)
        {
            Type type;

            if (!TryGetDefinition(itemName, out type))
            {
                result = null;
                return false;
            }

            result = (TBaseType) parentFactory.CreateInstance(type);
            return true;
        }

        /// <summary>
        ///     Creates an item instance.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <returns>Created item.</returns>
        public TBaseType CreateInstance(string name)
        {
            TBaseType result;

            if (TryCreateInstance(name, out result))
            {
                return result;
            }

            throw new ArgumentException(typeof (TBaseType).Name + " cannot be found: '" + name + "'");
        }

        /// <summary>
        ///     Registers the item based on a type name.
        /// </summary>
        /// <param name="itemName">Name of the item.</param>
        /// <param name="typeName">Name of the type.</param>
        public void RegisterNamedType(string itemName, string typeName)
        {
            items[itemName] = () => Type.GetType(typeName, false);
        }

        private delegate Type GetTypeDelegate();
    }
}