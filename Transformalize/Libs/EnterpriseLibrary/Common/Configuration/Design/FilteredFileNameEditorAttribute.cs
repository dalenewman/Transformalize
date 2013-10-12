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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{
    /// <summary>
    /// Specifies additional metadata for the FilteredFileNameEditor editor.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments"), AttributeUsage(AttributeTargets.Property)]
    public sealed class FilteredFileNameEditorAttribute : Attribute
    {
        private string filter;

        /// <summary>
        /// Initialize a new instance of the <see cref="FilteredFileNameEditorAttribute"/> class with the <see cref="Type"/> containing the resources and the resource key.
        /// </summary>
        /// <param name="resourceType">The <see cref="Type"/> containing the resources.</param>
        /// <param name="resourceKey">The resource key.</param>
        public FilteredFileNameEditorAttribute(Type resourceType, string resourceKey)
        {
            if (null == resourceType) throw new ArgumentNullException("resourceType");

            this.filter = ResourceStringLoader.LoadString(resourceType.FullName, resourceKey, resourceType.Assembly);
            this.CheckFileExists = true;
        }

        /// <summary>
        /// Gets the filter for the dialog.
        /// </summary>
        /// <value>
        /// The filter for the dialog.
        /// </value>
        public string Filter
        {
            get { return this.filter; }
        }

        /// <summary>
        /// Gets or sets whether the Open File Dialog should only allow existing files to be selected.
        /// </summary>
        /// <value>
        /// <see langword="true"/> if the Open File Dialog is used to open existing files. Otherwise <see langword="false"/>.
        /// </value>
        public bool CheckFileExists
        {
            get;
            set;
        }
    }
}
