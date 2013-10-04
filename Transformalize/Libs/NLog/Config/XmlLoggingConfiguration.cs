#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using Transformalize.Libs.NLog.Common;
using Transformalize.Libs.NLog.Internal;
using Transformalize.Libs.NLog.Layouts;
using Transformalize.Libs.NLog.Targets;
using Transformalize.Libs.NLog.Targets.Wrappers;

namespace Transformalize.Libs.NLog.Config
{
    /// <summary>
    ///     A class for configuring NLog through an XML configuration file
    ///     (App.config style or App.nlog style).
    /// </summary>
    public class XmlLoggingConfiguration : LoggingConfiguration
    {
        private readonly ConfigurationItemFactory configurationItemFactory = ConfigurationItemFactory.Default;
        private readonly Dictionary<string, bool> visitedFile = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string> variables = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private string originalFileName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlLoggingConfiguration" /> class.
        /// </summary>
        /// <param name="fileName">Configuration file to be read.</param>
        public XmlLoggingConfiguration(string fileName)
        {
            using (var reader = XmlReader.Create(fileName))
            {
                Initialize(reader, fileName, false);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlLoggingConfiguration" /> class.
        /// </summary>
        /// <param name="fileName">Configuration file to be read.</param>
        /// <param name="ignoreErrors">Ignore any errors during configuration.</param>
        public XmlLoggingConfiguration(string fileName, bool ignoreErrors)
        {
            using (var reader = XmlReader.Create(fileName))
            {
                Initialize(reader, fileName, ignoreErrors);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlLoggingConfiguration" /> class.
        /// </summary>
        /// <param name="reader">
        ///     <see cref="XmlReader" /> containing the configuration section.
        /// </param>
        /// <param name="fileName">Name of the file that contains the element (to be used as a base for including other files).</param>
        public XmlLoggingConfiguration(XmlReader reader, string fileName)
        {
            Initialize(reader, fileName, false);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlLoggingConfiguration" /> class.
        /// </summary>
        /// <param name="reader">
        ///     <see cref="XmlReader" /> containing the configuration section.
        /// </param>
        /// <param name="fileName">Name of the file that contains the element (to be used as a base for including other files).</param>
        /// <param name="ignoreErrors">Ignore any errors during configuration.</param>
        public XmlLoggingConfiguration(XmlReader reader, string fileName, bool ignoreErrors)
        {
            Initialize(reader, fileName, ignoreErrors);
        }

#if !SILVERLIGHT
        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlLoggingConfiguration" /> class.
        /// </summary>
        /// <param name="element">The XML element.</param>
        /// <param name="fileName">Name of the XML file.</param>
        internal XmlLoggingConfiguration(XmlElement element, string fileName)
        {
            using (var stringReader = new StringReader(element.OuterXml))
            {
                var reader = XmlReader.Create(stringReader);

                Initialize(reader, fileName, false);
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="XmlLoggingConfiguration" /> class.
        /// </summary>
        /// <param name="element">The XML element.</param>
        /// <param name="fileName">Name of the XML file.</param>
        /// <param name="ignoreErrors">
        ///     If set to <c>true</c> errors will be ignored during file processing.
        /// </param>
        internal XmlLoggingConfiguration(XmlElement element, string fileName, bool ignoreErrors)
        {
            using (var stringReader = new StringReader(element.OuterXml))
            {
                var reader = XmlReader.Create(stringReader);

                Initialize(reader, fileName, ignoreErrors);
            }
        }
#endif

#if !NET_CF && !SILVERLIGHT
        /// <summary>
        ///     Gets the default <see cref="LoggingConfiguration" /> object by parsing
        ///     the application configuration file (<c>app.exe.config</c>).
        /// </summary>
        public static LoggingConfiguration AppConfig
        {
            get
            {
                var o = ConfigurationManager.GetSection("nlog");
                return o as LoggingConfiguration;
            }
        }
#endif

        /// <summary>
        ///     Gets or sets a value indicating whether the configuration files
        ///     should be watched for changes and reloaded automatically when changed.
        /// </summary>
        public bool AutoReload { get; set; }

        /// <summary>
        ///     Gets the collection of file names which should be watched for changes by NLog.
        ///     This is the list of configuration files processed.
        ///     If the <c>autoReload</c> attribute is not set it returns empty collection.
        /// </summary>
        public override IEnumerable<string> FileNamesToWatch
        {
            get
            {
                if (AutoReload)
                {
                    return visitedFile.Keys;
                }

                return new string[0];
            }
        }

        /// <summary>
        ///     Re-reads the original configuration file and returns the new <see cref="LoggingConfiguration" /> object.
        /// </summary>
        /// <returns>
        ///     The new <see cref="XmlLoggingConfiguration" /> object.
        /// </returns>
        public override LoggingConfiguration Reload()
        {
            return new XmlLoggingConfiguration(originalFileName);
        }

        private static bool IsTargetElement(string name)
        {
            return name.Equals("target", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("wrapper", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("wrapper-target", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("compound-target", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsTargetRefElement(string name)
        {
            return name.Equals("target-ref", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("wrapper-target-ref", StringComparison.OrdinalIgnoreCase)
                   || name.Equals("compound-target-ref", StringComparison.OrdinalIgnoreCase);
        }

        private static string CleanWhitespace(string s)
        {
            s = s.Replace(" ", string.Empty); // get rid of the whitespace
            return s;
        }

        private static string StripOptionalNamespacePrefix(string attributeValue)
        {
            if (attributeValue == null)
            {
                return null;
            }

            var p = attributeValue.IndexOf(':');
            if (p < 0)
            {
                return attributeValue;
            }

            return attributeValue.Substring(p + 1);
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Target is disposed elsewhere.")]
        private static Target WrapWithAsyncTargetWrapper(Target target)
        {
            var asyncTargetWrapper = new AsyncTargetWrapper();
            asyncTargetWrapper.WrappedTarget = target;
            asyncTargetWrapper.Name = target.Name;
            target.Name = target.Name + "_wrapped";
            InternalLogger.Debug("Wrapping target '{0}' with AsyncTargetWrapper and renaming to '{1}", asyncTargetWrapper.Name, target.Name);
            target = asyncTargetWrapper;
            return target;
        }

        /// <summary>
        ///     Initializes the configuration.
        /// </summary>
        /// <param name="reader">
        ///     <see cref="XmlReader" /> containing the configuration section.
        /// </param>
        /// <param name="fileName">Name of the file that contains the element (to be used as a base for including other files).</param>
        /// <param name="ignoreErrors">Ignore any errors during configuration.</param>
        private void Initialize(XmlReader reader, string fileName, bool ignoreErrors)
        {
            try
            {
                reader.MoveToContent();
                var content = new NLogXmlElement(reader);
                if (fileName != null)
                {
                    InternalLogger.Info("Configuring from an XML element in {0}...", fileName);
#if SILVERLIGHT
                    string key = fileName;
#else
                    var key = Path.GetFullPath(fileName);
#endif
                    visitedFile[key] = true;

                    originalFileName = fileName;
                    ParseTopLevel(content, Path.GetDirectoryName(fileName));
                }
                else
                {
                    ParseTopLevel(content, null);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("Error {0}...", exception);
                if (!ignoreErrors)
                {
                    throw new NLogConfigurationException("Exception occurred when loading configuration from " + fileName, exception);
                }
            }
        }

        private void ConfigureFromFile(string fileName)
        {
#if SILVERLIGHT
    // file names are relative to XAP
            string key = fileName;
#else
            var key = Path.GetFullPath(fileName);
#endif
            if (visitedFile.ContainsKey(key))
            {
                return;
            }

            visitedFile[key] = true;

            ParseTopLevel(new NLogXmlElement(fileName), Path.GetDirectoryName(fileName));
        }

        private void ParseTopLevel(NLogXmlElement content, string baseDirectory)
        {
            content.AssertName("nlog", "configuration");

            switch (content.LocalName.ToUpper(CultureInfo.InvariantCulture))
            {
                case "CONFIGURATION":
                    ParseConfigurationElement(content, baseDirectory);
                    break;

                case "NLOG":
                    ParseNLogElement(content, baseDirectory);
                    break;
            }
        }

        private void ParseConfigurationElement(NLogXmlElement configurationElement, string baseDirectory)
        {
            InternalLogger.Trace("ParseConfigurationElement");
            configurationElement.AssertName("configuration");

            foreach (var el in configurationElement.Elements("nlog"))
            {
                ParseNLogElement(el, baseDirectory);
            }
        }

        private void ParseNLogElement(NLogXmlElement nlogElement, string baseDirectory)
        {
            InternalLogger.Trace("ParseNLogElement");
            nlogElement.AssertName("nlog");

            AutoReload = nlogElement.GetOptionalBooleanAttribute("autoReload", false);
            LogManager.ThrowExceptions = nlogElement.GetOptionalBooleanAttribute("throwExceptions", LogManager.ThrowExceptions);
            InternalLogger.LogToConsole = nlogElement.GetOptionalBooleanAttribute("internalLogToConsole", InternalLogger.LogToConsole);
#if !NET_CF
            InternalLogger.LogToConsoleError = nlogElement.GetOptionalBooleanAttribute("internalLogToConsoleError", InternalLogger.LogToConsoleError);
#endif
            InternalLogger.LogFile = nlogElement.GetOptionalAttribute("internalLogFile", InternalLogger.LogFile);
            InternalLogger.LogLevel = LogLevel.FromString(nlogElement.GetOptionalAttribute("internalLogLevel", InternalLogger.LogLevel.Name));
            LogManager.GlobalThreshold = LogLevel.FromString(nlogElement.GetOptionalAttribute("globalThreshold", LogManager.GlobalThreshold.Name));

            foreach (var el in nlogElement.Children)
            {
                switch (el.LocalName.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "EXTENSIONS":
                        ParseExtensionsElement(el, baseDirectory);
                        break;

                    case "INCLUDE":
                        ParseIncludeElement(el, baseDirectory);
                        break;

                    case "APPENDERS":
                    case "TARGETS":
                        ParseTargetsElement(el);
                        break;

                    case "VARIABLE":
                        ParseVariableElement(el);
                        break;

                    case "RULES":
                        ParseRulesElement(el, LoggingRules);
                        break;

                    default:
                        InternalLogger.Warn("Skipping unknown node: {0}", el.LocalName);
                        break;
                }
            }
        }

        private void ParseRulesElement(NLogXmlElement rulesElement, IList<LoggingRule> rulesCollection)
        {
            InternalLogger.Trace("ParseRulesElement");
            rulesElement.AssertName("rules");

            foreach (var loggerElement in rulesElement.Elements("logger"))
            {
                ParseLoggerElement(loggerElement, rulesCollection);
            }
        }

        private void ParseLoggerElement(NLogXmlElement loggerElement, IList<LoggingRule> rulesCollection)
        {
            loggerElement.AssertName("logger");

            var rule = new LoggingRule();
            var namePattern = loggerElement.GetOptionalAttribute("name", "*");
            var appendTo = loggerElement.GetOptionalAttribute("appendTo", null);
            if (appendTo == null)
            {
                appendTo = loggerElement.GetOptionalAttribute("writeTo", null);
            }

            rule.LoggerNamePattern = namePattern;
            if (appendTo != null)
            {
                foreach (var t in appendTo.Split(','))
                {
                    var targetName = t.Trim();
                    var target = FindTargetByName(targetName);

                    if (target != null)
                    {
                        rule.Targets.Add(target);
                    }
                    else
                    {
                        throw new NLogConfigurationException("Target " + targetName + " not found.");
                    }
                }
            }

            rule.Final = loggerElement.GetOptionalBooleanAttribute("final", false);

            string levelString;

            if (loggerElement.AttributeValues.TryGetValue("level", out levelString))
            {
                var level = LogLevel.FromString(levelString);
                rule.EnableLoggingForLevel(level);
            }
            else if (loggerElement.AttributeValues.TryGetValue("levels", out levelString))
            {
                levelString = CleanWhitespace(levelString);

                var tokens = levelString.Split(',');
                foreach (var s in tokens)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        var level = LogLevel.FromString(s);
                        rule.EnableLoggingForLevel(level);
                    }
                }
            }
            else
            {
                var minLevel = 0;
                var maxLevel = LogLevel.MaxLevel.Ordinal;
                string minLevelString;
                string maxLevelString;

                if (loggerElement.AttributeValues.TryGetValue("minLevel", out minLevelString))
                {
                    minLevel = LogLevel.FromString(minLevelString).Ordinal;
                }

                if (loggerElement.AttributeValues.TryGetValue("maxLevel", out maxLevelString))
                {
                    maxLevel = LogLevel.FromString(maxLevelString).Ordinal;
                }

                for (var i = minLevel; i <= maxLevel; ++i)
                {
                    rule.EnableLoggingForLevel(LogLevel.FromOrdinal(i));
                }
            }

            foreach (var child in loggerElement.Children)
            {
                switch (child.LocalName.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "FILTERS":
                        ParseFilters(rule, child);
                        break;

                    case "LOGGER":
                        ParseLoggerElement(child, rule.ChildRules);
                        break;
                }
            }

            rulesCollection.Add(rule);
        }

        private void ParseFilters(LoggingRule rule, NLogXmlElement filtersElement)
        {
            filtersElement.AssertName("filters");

            foreach (var filterElement in filtersElement.Children)
            {
                var name = filterElement.LocalName;

                var filter = configurationItemFactory.Filters.CreateInstance(name);
                ConfigureObjectFromAttributes(filter, filterElement, false);
                rule.Filters.Add(filter);
            }
        }

        private void ParseVariableElement(NLogXmlElement variableElement)
        {
            variableElement.AssertName("variable");

            var name = variableElement.GetRequiredAttribute("name");
            var value = ExpandVariables(variableElement.GetRequiredAttribute("value"));

            variables[name] = value;
        }

        private void ParseTargetsElement(NLogXmlElement targetsElement)
        {
            targetsElement.AssertName("targets", "appenders");

            var asyncWrap = targetsElement.GetOptionalBooleanAttribute("async", false);
            NLogXmlElement defaultWrapperElement = null;
            var typeNameToDefaultTargetParameters = new Dictionary<string, NLogXmlElement>();

            foreach (var targetElement in targetsElement.Children)
            {
                var name = targetElement.LocalName;
                var type = StripOptionalNamespacePrefix(targetElement.GetOptionalAttribute("type", null));

                switch (name.ToUpper(CultureInfo.InvariantCulture))
                {
                    case "DEFAULT-WRAPPER":
                        defaultWrapperElement = targetElement;
                        break;

                    case "DEFAULT-TARGET-PARAMETERS":
                        if (type == null)
                        {
                            throw new NLogConfigurationException("Missing 'type' attribute on <" + name + "/>.");
                        }

                        typeNameToDefaultTargetParameters[type] = targetElement;
                        break;

                    case "TARGET":
                    case "APPENDER":
                    case "WRAPPER":
                    case "WRAPPER-TARGET":
                    case "COMPOUND-TARGET":
                        if (type == null)
                        {
                            throw new NLogConfigurationException("Missing 'type' attribute on <" + name + "/>.");
                        }

                        var newTarget = configurationItemFactory.Targets.CreateInstance(type);

                        NLogXmlElement defaults;
                        if (typeNameToDefaultTargetParameters.TryGetValue(type, out defaults))
                        {
                            ParseTargetElement(newTarget, defaults);
                        }

                        ParseTargetElement(newTarget, targetElement);

                        if (asyncWrap)
                        {
                            newTarget = WrapWithAsyncTargetWrapper(newTarget);
                        }

                        if (defaultWrapperElement != null)
                        {
                            newTarget = WrapWithDefaultWrapper(newTarget, defaultWrapperElement);
                        }

                        InternalLogger.Info("Adding target {0}", newTarget);
                        AddTarget(newTarget.Name, newTarget);
                        break;
                }
            }
        }

        private void ParseTargetElement(Target target, NLogXmlElement targetElement)
        {
            var compound = target as CompoundTargetBase;
            var wrapper = target as WrapperTargetBase;

            ConfigureObjectFromAttributes(target, targetElement, true);

            foreach (var childElement in targetElement.Children)
            {
                var name = childElement.LocalName;

                if (compound != null)
                {
                    if (IsTargetRefElement(name))
                    {
                        var targetName = childElement.GetRequiredAttribute("name");
                        var newTarget = FindTargetByName(targetName);
                        if (newTarget == null)
                        {
                            throw new NLogConfigurationException("Referenced target '" + targetName + "' not found.");
                        }

                        compound.Targets.Add(newTarget);
                        continue;
                    }

                    if (IsTargetElement(name))
                    {
                        var type = StripOptionalNamespacePrefix(childElement.GetRequiredAttribute("type"));

                        var newTarget = configurationItemFactory.Targets.CreateInstance(type);
                        if (newTarget != null)
                        {
                            ParseTargetElement(newTarget, childElement);
                            if (newTarget.Name != null)
                            {
                                // if the new target has name, register it
                                AddTarget(newTarget.Name, newTarget);
                            }

                            compound.Targets.Add(newTarget);
                        }

                        continue;
                    }
                }

                if (wrapper != null)
                {
                    if (IsTargetRefElement(name))
                    {
                        var targetName = childElement.GetRequiredAttribute("name");
                        var newTarget = FindTargetByName(targetName);
                        if (newTarget == null)
                        {
                            throw new NLogConfigurationException("Referenced target '" + targetName + "' not found.");
                        }

                        wrapper.WrappedTarget = newTarget;
                        continue;
                    }

                    if (IsTargetElement(name))
                    {
                        var type = StripOptionalNamespacePrefix(childElement.GetRequiredAttribute("type"));

                        var newTarget = configurationItemFactory.Targets.CreateInstance(type);
                        if (newTarget != null)
                        {
                            ParseTargetElement(newTarget, childElement);
                            if (newTarget.Name != null)
                            {
                                // if the new target has name, register it
                                AddTarget(newTarget.Name, newTarget);
                            }

                            if (wrapper.WrappedTarget != null)
                            {
                                throw new NLogConfigurationException("Wrapped target already defined.");
                            }

                            wrapper.WrappedTarget = newTarget;
                        }

                        continue;
                    }
                }

                SetPropertyFromElement(target, childElement);
            }
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFrom", Justification = "Need to load external assembly.")]
        private void ParseExtensionsElement(NLogXmlElement extensionsElement, string baseDirectory)
        {
            extensionsElement.AssertName("extensions");

            foreach (var addElement in extensionsElement.Elements("add"))
            {
                var prefix = addElement.GetOptionalAttribute("prefix", null);

                if (prefix != null)
                {
                    prefix = prefix + ".";
                }

                var type = StripOptionalNamespacePrefix(addElement.GetOptionalAttribute("type", null));
                if (type != null)
                {
                    configurationItemFactory.RegisterType(Type.GetType(type, true), prefix);
                }

#if !WINDOWS_PHONE
                var assemblyFile = addElement.GetOptionalAttribute("assemblyFile", null);
                if (assemblyFile != null)
                {
                    try
                    {
#if SILVERLIGHT
                                var si = Application.GetResourceStream(new Uri(assemblyFile, UriKind.Relative));
                                var assemblyPart = new AssemblyPart();
                                Assembly asm = assemblyPart.Load(si.Stream);
#else

                        var fullFileName = Path.Combine(baseDirectory, assemblyFile);
                        InternalLogger.Info("Loading assembly file: {0}", fullFileName);

                        var asm = Assembly.LoadFrom(fullFileName);
#endif
                        configurationItemFactory.RegisterItemsFromAssembly(asm, prefix);
                    }
                    catch (Exception exception)
                    {
                        if (exception.MustBeRethrown())
                        {
                            throw;
                        }

                        InternalLogger.Error("Error loading extensions: {0}", exception);
                        if (LogManager.ThrowExceptions)
                        {
                            throw new NLogConfigurationException("Error loading extensions: " + assemblyFile, exception);
                        }
                    }

                    continue;
                }

                var assemblyName = addElement.GetOptionalAttribute("assembly", null);
                if (assemblyName != null)
                {
                    try
                    {
                        InternalLogger.Info("Loading assembly name: {0}", assemblyName);
#if SILVERLIGHT
                        var si = Application.GetResourceStream(new Uri(assemblyName + ".dll", UriKind.Relative));
                        var assemblyPart = new AssemblyPart();
                        Assembly asm = assemblyPart.Load(si.Stream);
#else
                        var asm = Assembly.Load(assemblyName);
#endif

                        configurationItemFactory.RegisterItemsFromAssembly(asm, prefix);
                    }
                    catch (Exception exception)
                    {
                        if (exception.MustBeRethrown())
                        {
                            throw;
                        }

                        InternalLogger.Error("Error loading extensions: {0}", exception);
                        if (LogManager.ThrowExceptions)
                        {
                            throw new NLogConfigurationException("Error loading extensions: " + assemblyName, exception);
                        }
                    }

                    continue;
                }
#endif
            }
        }

        private void ParseIncludeElement(NLogXmlElement includeElement, string baseDirectory)
        {
            includeElement.AssertName("include");

            var newFileName = includeElement.GetRequiredAttribute("file");

            try
            {
                newFileName = ExpandVariables(newFileName);
                newFileName = SimpleLayout.Evaluate(newFileName);
                if (baseDirectory != null)
                {
                    newFileName = Path.Combine(baseDirectory, newFileName);
                }

#if SILVERLIGHT
                newFileName = newFileName.Replace("\\", "/");
                if (Application.GetResourceStream(new Uri(newFileName, UriKind.Relative)) != null)
#else
                if (File.Exists(newFileName))
#endif
                {
                    InternalLogger.Debug("Including file '{0}'", newFileName);
                    ConfigureFromFile(newFileName);
                }
                else
                {
                    throw new FileNotFoundException("Included file not found: " + newFileName);
                }
            }
            catch (Exception exception)
            {
                if (exception.MustBeRethrown())
                {
                    throw;
                }

                InternalLogger.Error("Error when including '{0}' {1}", newFileName, exception);

                if (includeElement.GetOptionalBooleanAttribute("ignoreErrors", false))
                {
                    return;
                }

                throw new NLogConfigurationException("Error when including: " + newFileName, exception);
            }
        }

        private void SetPropertyFromElement(object o, NLogXmlElement element)
        {
            if (AddArrayItemFromElement(o, element))
            {
                return;
            }

            if (SetLayoutFromElement(o, element))
            {
                return;
            }

            PropertyHelper.SetPropertyFromString(o, element.LocalName, ExpandVariables(element.Value), configurationItemFactory);
        }

        private bool AddArrayItemFromElement(object o, NLogXmlElement element)
        {
            var name = element.LocalName;

            PropertyInfo propInfo;
            if (!PropertyHelper.TryGetPropertyInfo(o, name, out propInfo))
            {
                return false;
            }

            var elementType = PropertyHelper.GetArrayItemType(propInfo);
            if (elementType != null)
            {
                var propertyValue = (IList) propInfo.GetValue(o, null);
                var arrayItem = FactoryHelper.CreateInstance(elementType);
                ConfigureObjectFromAttributes(arrayItem, element, true);
                ConfigureObjectFromElement(arrayItem, element);
                propertyValue.Add(arrayItem);
                return true;
            }

            return false;
        }

        private void ConfigureObjectFromAttributes(object targetObject, NLogXmlElement element, bool ignoreType)
        {
            foreach (var kvp in element.AttributeValues)
            {
                var childName = kvp.Key;
                var childValue = kvp.Value;

                if (ignoreType && childName.Equals("type", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                PropertyHelper.SetPropertyFromString(targetObject, childName, ExpandVariables(childValue), configurationItemFactory);
            }
        }

        private bool SetLayoutFromElement(object o, NLogXmlElement layoutElement)
        {
            PropertyInfo targetPropertyInfo;
            var name = layoutElement.LocalName;

            // if property exists
            if (PropertyHelper.TryGetPropertyInfo(o, name, out targetPropertyInfo))
            {
                // and is a Layout
                if (typeof (Layout).IsAssignableFrom(targetPropertyInfo.PropertyType))
                {
                    var layoutTypeName = StripOptionalNamespacePrefix(layoutElement.GetOptionalAttribute("type", null));

                    // and 'type' attribute has been specified
                    if (layoutTypeName != null)
                    {
                        // configure it from current element
                        var layout = configurationItemFactory.Layouts.CreateInstance(ExpandVariables(layoutTypeName));
                        ConfigureObjectFromAttributes(layout, layoutElement, true);
                        ConfigureObjectFromElement(layout, layoutElement);
                        targetPropertyInfo.SetValue(o, layout, null);
                        return true;
                    }
                }
            }

            return false;
        }

        private void ConfigureObjectFromElement(object targetObject, NLogXmlElement element)
        {
            foreach (var child in element.Children)
            {
                SetPropertyFromElement(targetObject, child);
            }
        }

        private Target WrapWithDefaultWrapper(Target t, NLogXmlElement defaultParameters)
        {
            var wrapperType = StripOptionalNamespacePrefix(defaultParameters.GetRequiredAttribute("type"));

            var wrapperTargetInstance = configurationItemFactory.Targets.CreateInstance(wrapperType);
            var wtb = wrapperTargetInstance as WrapperTargetBase;
            if (wtb == null)
            {
                throw new NLogConfigurationException("Target type specified on <default-wrapper /> is not a wrapper.");
            }

            ParseTargetElement(wrapperTargetInstance, defaultParameters);
            while (wtb.WrappedTarget != null)
            {
                wtb = wtb.WrappedTarget as WrapperTargetBase;
                if (wtb == null)
                {
                    throw new NLogConfigurationException("Child target type specified on <default-wrapper /> is not a wrapper.");
                }
            }

            wtb.WrappedTarget = t;
            wrapperTargetInstance.Name = t.Name;
            t.Name = t.Name + "_wrapped";

            InternalLogger.Debug("Wrapping target '{0}' with '{1}' and renaming to '{2}", wrapperTargetInstance.Name, wrapperTargetInstance.GetType().Name, t.Name);
            return wrapperTargetInstance;
        }

        private string ExpandVariables(string input)
        {
            var output = input;

            // TODO - make this case-insensitive, will probably require a different approach
            foreach (var kvp in variables)
            {
                output = output.Replace("${" + kvp.Key + "}", kvp.Value);
            }

            return output;
        }
    }
}