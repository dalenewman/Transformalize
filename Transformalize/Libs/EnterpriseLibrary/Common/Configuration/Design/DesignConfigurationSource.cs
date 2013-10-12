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

using System;
using System.IO;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{
    /// <summary>
    /// configuration source to support design-time configuration of <see cref="FileConfigurationSource"/>
    /// </summary>
    public class DesignConfigurationSource : FileConfigurationSource, IDesignConfigurationSource
    {
        
        ///<summary>
        /// Initializes a new instance of <see cref="DesignConfigurationSource"/> based on file path.
        ///</summary>
        ///<param name="configurationFilePath"></param>
        public DesignConfigurationSource(string configurationFilePath)
            :base(configurationFilePath)
        {
        }

        ///<summary>
        /// Retrieves a local section from the configuration source.
        ///</summary>
        ///<param name="sectionName"></param>
        ///<returns>The configuration section or null if it does not contain the section.</returns>
        public System.Configuration.ConfigurationSection GetLocalSection(string sectionName)
        {
            return DoGetSection(sectionName);
        }

        /// <summary>
        /// Adds a local section to the configuration source.
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="section"></param>
        public void AddLocalSection(string sectionName, System.Configuration.ConfigurationSection section)
        {
            DoAdd(sectionName, section);
        }

        ///<summary>
        /// Removes a local section from the configuration source.
        ///</summary>
        ///<param name="sectionName"></param>
        public void RemoveLocalSection(string sectionName)
        {
            DoRemove(sectionName);
        }

        /// <summary>
        /// Gets the path of the configuration file for the configuration source.
        /// </summary>
        public new string ConfigurationFilePath
        {
            get { return base.ConfigurationFilePath; }
        }


        /// <summary>
        /// Creates a new instance of <see cref="DesignConfigurationSource"/> based on <paramref name="rootSource"/> and <paramref name="filePath"/>.
        /// </summary>
        /// <param name="rootSource">The source that was used to open the main conifguration file.</param>
        /// <param name="filePath">An absolute of relative path to the file to which the source should be created.</param>
        /// <returns>A new instance of <see cref="DesignConfigurationSource"/>.</returns>
        public static IDesignConfigurationSource CreateDesignSource(IDesignConfigurationSource rootSource, string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException(Resources.ExceptionStringNullOrEmpty);

            DesignConfigurationSource rootSourceAsDesignSource = rootSource as DesignConfigurationSource;
            if (rootSourceAsDesignSource == null)
                throw new ArgumentException(Resources.CannotCreateDesignSource, "rootSource");

            {
                string mainConfigurationFileDirectory =
                    Path.GetDirectoryName(rootSourceAsDesignSource.ConfigurationFilePath);

                string fullFilePath = Path.Combine(mainConfigurationFileDirectory, filePath);

                if (!File.Exists(fullFilePath))
                {
                    File.WriteAllText(fullFilePath, @"<configuration />");
                }

                return new DesignConfigurationSource(fullFilePath);
            }
        }
    }
}
