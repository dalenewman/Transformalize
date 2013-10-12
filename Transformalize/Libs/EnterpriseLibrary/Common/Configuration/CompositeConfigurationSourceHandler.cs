//===============================================================================
// Microsoft patterns & practices Enterprise Library
// Core
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================

using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Implements compositional merge behavior to <see cref="IConfigurationSource"/> implementations.<br/>
    /// </summary>
    /// <seealso cref="IConfigurationSource"/>
    /// <seealso cref="ConfigurationSourceHandler"/>
    public class CompositeConfigurationSourceHandler : ConfigurationSourceHandler
    {
        Dictionary<string, string> sectionRedirectTable = new Dictionary<string, string>();
        readonly IConfigurationSource mainConfigurationSource;

        /// <summary>
        /// Creates a new instance of <see cref="CompositeConfigurationSourceHandler"/>.
        /// </summary>
        /// <param name="mainConfigurationSource">The <see cref="IConfigurationSource"/> instance that should be extended.</param>
        public CompositeConfigurationSourceHandler(IConfigurationSource mainConfigurationSource)
            : base(mainConfigurationSource)
        {
            this.mainConfigurationSource = mainConfigurationSource;
        }

        /// <summary>
        /// Performs intialization logic for this <see cref="CompositeConfigurationSourceHandler"/>.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            sectionRedirectTable = GetSectionRedirectTable();
        }

        /// <summary>
        /// Performs re-intialization logic for this <see cref="CompositeConfigurationSourceHandler"/>.
        /// </summary>
        protected override void DoRefresh()
        {
            base.DoRefresh();
            sectionRedirectTable = GetSectionRedirectTable();
        }

        private Dictionary<string, string> GetSectionRedirectTable()
        {
            ConfigurationSourceSection sourcesSection = mainConfigurationSource.GetSection(ConfigurationSourceSection.SectionName) as ConfigurationSourceSection;
            if (sourcesSection != null)
            {
                return sourcesSection.RedirectedSections.ToDictionary(x => x.Name, x => x.SourceName);
            }

            return new Dictionary<string, string>();
        }

        /// <summary>
        /// Checks whether the result of a call to <see cref="IConfigurationSource.GetSection(string)"/> should be deferred to a subordinate source.<br/>
        /// If the call should be deferred, returns the <see cref="ConfigurationSection"/> intance from the approriate source.<br/>
        /// If the call should not be deferred returns <paramref name="configurationSection"/>.
        /// </summary>
        /// <param name="sectionName">The name of the section that was retrieved from configuration.</param>
        /// <param name="configurationSection">The section that was retrieved from configuration.</param>
        /// <returns>The resulting <see cref="ConfigurationSection"/> instance.</returns>
        /// <seealso cref="IConfigurationSource.GetSection(string)"/>
        /// <exception cref="ConfigurationSourceErrorsException">Thrown if a section does not exist in a registered source.</exception>
        protected override ConfigurationSection DoCheckGetSection(string sectionName, ConfigurationSection configurationSection)
        {
            string sourceNameForSection;
            if (!sectionRedirectTable.TryGetValue(sectionName, out sourceNameForSection))
            {
                return configurationSection;
            }

            //if no source is specified we can return.
            if (string.IsNullOrEmpty(sourceNameForSection))
            {
                return configurationSection;
            }

            IConfigurationSource subordinateSource = GetSubordinateSource(sourceNameForSection);

            EnsurePropagatingSectionChangeEvents(sourceNameForSection, sectionName);

            var section = subordinateSource.GetSection(sectionName);

            if (section == null)
                throw new ConfigurationSourceErrorsException(
                    string.Format(CultureInfo.CurrentCulture,
                    Resources.ExceptionRedirectedConfigurationSectionNotFound,
                    sectionName,
                    sourceNameForSection));

            return section;
        }

        /// <summary>
        /// Checks whether a call to <see cref="IConfigurationSource.Add(string, ConfigurationSection)"/> should be deferred to a subordinate source.<br/>
        /// If the call should be deferred, adds the <paramref name="configurationSection"/> to the appropriate source and returns <see langword="true"/>.<br/>
        /// If the call should not be deferred returns <see langword="true"/>.
        /// </summary>
        /// <param name="sectionName">The name of the section that should be added to configuration.</param>
        /// <param name="configurationSection">The section that should be added to configuration.</param>
        /// <returns><see langword="true"/> if the section was added in a subordinate source, otherwise <see langword="false"/>.</returns>
        /// <seealso cref="IConfigurationSource.Add(string, ConfigurationSection)"/>
        protected override bool DoCheckAddSection(string sectionName, ConfigurationSection configurationSection)
        {
            string sourceNameForSection;
            if (sectionRedirectTable.TryGetValue(sectionName, out sourceNameForSection))
            {
                IConfigurationSource subordinateSource = GetSubordinateSource(sourceNameForSection);
                subordinateSource.Add(sectionName, configurationSection);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether a call to <see cref="IConfigurationSource.Remove(string)"/> should be deferred to a subordinate source.<br/>
        /// If the call should be deferred, removes the section from the appropriate source and returns <see langword="true"/>.<br/>
        /// If the call should not be deferred returns <see langword="true"/>.
        /// </summary>
        /// <param name="sectionName">The name of the section that should be removed from configuration.</param>
        /// <returns><see langword="true"/> if the section was removed from a subordinate source, otherwise <see langword="false"/>.</returns>
        /// <seealso cref="IConfigurationSource.Remove(string)"/>
        protected override bool DoCheckRemoveSection(string sectionName)
        {
            string sourceNameForSection;
            if (sectionRedirectTable.TryGetValue(sectionName, out sourceNameForSection))
            {
                IConfigurationSource subordinateSource = GetSubordinateSource(sourceNameForSection);
                subordinateSource.Remove(sectionName);
                return true;
            }
            return false;
        }
    }
}
