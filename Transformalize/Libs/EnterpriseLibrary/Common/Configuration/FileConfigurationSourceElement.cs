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

using System.ComponentModel;
using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design;
using Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design.Validation;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration
{
    /// <summary>
    /// Represents the configuration settings that describe a <see cref="FileConfigurationSource"/>.
    /// </summary>
    [ResourceDescription(typeof(DesignResources), "FileConfigurationSourceElementDescription")]
    [ResourceDisplayName(typeof(DesignResources), "FileConfigurationSourceElementDisplayName")]
    [Browsable(true)]
    [EnvironmentalOverrides(false)]
    public class FileConfigurationSourceElement : ConfigurationSourceElement
    {
        private const string filePathProperty = "filePath";

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConfigurationSourceElement"/> class with a default name and an empty path.
        /// </summary>
        public FileConfigurationSourceElement()
            : this(Resources.FileConfigurationSourceName, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConfigurationSourceElement"/> class with a name and an path.
        /// </summary>
        /// <param name="name">The instance name.</param>
        /// <param name="filePath">The file path.</param>
        public FileConfigurationSourceElement(string name, string filePath)
            : base(name, typeof(FileConfigurationSource))
        {
            this.FilePath = filePath;
        }


        /// <summary>
        /// Gets or sets the file path. This is a required field.
        /// </summary>
        [ConfigurationProperty(filePathProperty, IsRequired = true)]
        [ResourceDescription(typeof(DesignResources), "FileConfigurationSourceElementFilePathDescription")]
        [ResourceDisplayName(typeof(DesignResources), "FileConfigurationSourceElementFilePathDisplayName")]
        [Editor(CommonDesignTime.EditorTypes.FilteredFilePath, CommonDesignTime.EditorTypes.UITypeEditor)]
        [FilteredFileNameEditor(typeof(DesignResources), "FileConfigurationSourceElementFilePathFilter", CheckFileExists = false)]
        [Validation(CommonDesignTime.ValidationTypeNames.PathExistsValidator)]
        [Validation(CommonDesignTime.ValidationTypeNames.FileWritableValidator)]
        public string FilePath
        {
            get { return (string)this[filePathProperty]; }
            set { this[filePathProperty] = value; }
        }

        /// <summary>
        /// Returns a new <see cref="FileConfigurationSource"/> configured with the receiver's settings.
        /// </summary>
        /// <returns>A new configuration source.</returns>
        public override IConfigurationSource CreateSource()
        {
            IConfigurationSource createdObject = new FileConfigurationSource(FilePath);

            return createdObject;
        }

        ///<summary>
        /// Returns a new <see cref="IDesignConfigurationSource"/> configured based on this configuration element.
        ///</summary>
        ///<returns>Returns a new <see cref="IDesignConfigurationSource"/> or null if this source does not have design-time support.</returns>
        public override IDesignConfigurationSource CreateDesignSource(IDesignConfigurationSource rootSource)
        {
            return DesignConfigurationSource.CreateDesignSource(rootSource, FilePath);


        }
    }
}
