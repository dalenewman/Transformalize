#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Reflection;
using Transformalize.Libs.NLog.Conditions;
using Transformalize.Libs.NLog.Filters;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.NLog.LayoutRenderers;
using Transformalize.Libs.NLog.Layouts;
using Transformalize.Libs.NLog.Targets;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     Provides registration information for named items (targets, layouts, layout renderers, etc.) managed by NLog.
    /// </summary>
    public class ConfigurationItemFactory
    {
        private readonly IList<object> allFactories;
        private readonly Factory<LayoutRenderer, AmbientPropertyAttribute> ambientProperties;
        private readonly MethodFactory<ConditionMethodsAttribute, ConditionMethodAttribute> conditionMethods;
        private readonly Factory<Filter, FilterAttribute> filters;
        private readonly Factory<LayoutRenderer, LayoutRendererAttribute> layoutRenderers;
        private readonly Factory<Layout, LayoutAttribute> layouts;
        private readonly Factory<Target, TargetAttribute> targets;

        /// <summary>
        ///     Initializes static members of the <see cref="ConfigurationItemFactory" /> class.
        /// </summary>
        static ConfigurationItemFactory()
        {
            Default = BuildDefaultFactory();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConfigurationItemFactory" /> class.
        /// </summary>
        /// <param name="assemblies">The assemblies to scan for named items.</param>
        public ConfigurationItemFactory(params Assembly[] assemblies)
        {
            CreateInstance = FactoryHelper.CreateInstance;
            targets = new Factory<Target, TargetAttribute>(this);
            filters = new Factory<Filter, FilterAttribute>(this);
            layoutRenderers = new Factory<LayoutRenderer, LayoutRendererAttribute>(this);
            layouts = new Factory<Layout, LayoutAttribute>(this);
            conditionMethods = new MethodFactory<ConditionMethodsAttribute, ConditionMethodAttribute>();
            ambientProperties = new Factory<LayoutRenderer, AmbientPropertyAttribute>(this);
            allFactories = new List<object>
                               {
                                   targets,
                                   filters,
                                   layoutRenderers,
                                   layouts,
                                   conditionMethods,
                                   ambientProperties,
                               };

            foreach (var asm in assemblies)
            {
                RegisterItemsFromAssembly(asm);
            }
        }

        /// <summary>
        ///     Gets or sets default singleton instance of <see cref="ConfigurationItemFactory" />.
        /// </summary>
        public static ConfigurationItemFactory Default { get; set; }

        /// <summary>
        ///     Gets or sets the creator delegate used to instantiate configuration objects.
        /// </summary>
        /// <remarks>
        ///     By overriding this property, one can enable dependency injection or interception for created objects.
        /// </remarks>
        public ConfigurationItemCreator CreateInstance { get; set; }

        /// <summary>
        ///     Gets the <see cref="Target" /> factory.
        /// </summary>
        /// <value>The target factory.</value>
        public INamedItemFactory<Target, Type> Targets
        {
            get { return targets; }
        }

        /// <summary>
        ///     Gets the <see cref="Filter" /> factory.
        /// </summary>
        /// <value>The filter factory.</value>
        public INamedItemFactory<Filter, Type> Filters
        {
            get { return filters; }
        }

        /// <summary>
        ///     Gets the <see cref="LayoutRenderer" /> factory.
        /// </summary>
        /// <value>The layout renderer factory.</value>
        public INamedItemFactory<LayoutRenderer, Type> LayoutRenderers
        {
            get { return layoutRenderers; }
        }

        /// <summary>
        ///     Gets the <see cref="LayoutRenderer" /> factory.
        /// </summary>
        /// <value>The layout factory.</value>
        public INamedItemFactory<Layout, Type> Layouts
        {
            get { return layouts; }
        }

        /// <summary>
        ///     Gets the ambient property factory.
        /// </summary>
        /// <value>The ambient property factory.</value>
        public INamedItemFactory<LayoutRenderer, Type> AmbientProperties
        {
            get { return ambientProperties; }
        }

        /// <summary>
        ///     Gets the condition method factory.
        /// </summary>
        /// <value>The condition method factory.</value>
        public INamedItemFactory<MethodInfo, MethodInfo> ConditionMethods
        {
            get { return conditionMethods; }
        }

        /// <summary>
        ///     Registers named items from the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void RegisterItemsFromAssembly(Assembly assembly)
        {
            RegisterItemsFromAssembly(assembly, string.Empty);
        }

        /// <summary>
        ///     Registers named items from the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="itemNamePrefix">Item name prefix.</param>
        public void RegisterItemsFromAssembly(Assembly assembly, string itemNamePrefix)
        {
            foreach (IFactory f in allFactories)
            {
                f.ScanAssembly(assembly, itemNamePrefix);
            }
        }

        /// <summary>
        ///     Clears the contents of all factories.
        /// </summary>
        public void Clear()
        {
            foreach (IFactory f in allFactories)
            {
                f.Clear();
            }
        }

        /// <summary>
        ///     Registers the type.
        /// </summary>
        /// <param name="type">The type to register.</param>
        /// <param name="itemNamePrefix">The item name prefix.</param>
        public void RegisterType(Type type, string itemNamePrefix)
        {
            foreach (IFactory f in allFactories)
            {
                f.RegisterType(type, itemNamePrefix);
            }
        }

        /// <summary>
        ///     Builds the default configuration item factory.
        /// </summary>
        /// <returns>Default factory.</returns>
        private static ConfigurationItemFactory BuildDefaultFactory()
        {
            var factory = new ConfigurationItemFactory(typeof (Logger).Assembly);
            factory.RegisterExtendedItems();

            return factory;
        }

        /// <summary>
        ///     Registers items in NLog.Extended.dll using late-bound types, so that we don't need a reference to NLog.Extended.dll.
        /// </summary>
        private void RegisterExtendedItems()
        {
            var suffix = typeof (Logger).AssemblyQualifiedName;
            var myAssemblyName = "NLog,";
            var extendedAssemblyName = "NLog.Extended,";
            var p = suffix.IndexOf(myAssemblyName, StringComparison.OrdinalIgnoreCase);
            if (p >= 0)
            {
                suffix = ", " + extendedAssemblyName + suffix.Substring(p + myAssemblyName.Length);

                // register types
                var targetsNamespace = typeof (DebugTarget).Namespace;
                targets.RegisterNamedType("AspNetTrace", targetsNamespace + ".AspNetTraceTarget" + suffix);
                targets.RegisterNamedType("MSMQ", targetsNamespace + ".MessageQueueTarget" + suffix);
                targets.RegisterNamedType("AspNetBufferingWrapper", targetsNamespace + ".Wrappers.AspNetBufferingTargetWrapper" + suffix);

                // register layout renderers
                var layoutRenderersNamespace = typeof (MessageLayoutRenderer).Namespace;
                layoutRenderers.RegisterNamedType("aspnet-application", layoutRenderersNamespace + ".AspNetApplicationValueLayoutRenderer" + suffix);
                layoutRenderers.RegisterNamedType("aspnet-request", layoutRenderersNamespace + ".AspNetRequestValueLayoutRenderer" + suffix);
                layoutRenderers.RegisterNamedType("aspnet-sessionid", layoutRenderersNamespace + ".AspNetSessionIDLayoutRenderer" + suffix);
                layoutRenderers.RegisterNamedType("aspnet-session", layoutRenderersNamespace + ".AspNetSessionValueLayoutRenderer" + suffix);
                layoutRenderers.RegisterNamedType("aspnet-user-authtype", layoutRenderersNamespace + ".AspNetUserAuthTypeLayoutRenderer" + suffix);
                layoutRenderers.RegisterNamedType("aspnet-user-identity", layoutRenderersNamespace + ".AspNetUserIdentityLayoutRenderer" + suffix);
            }
        }
    }
}