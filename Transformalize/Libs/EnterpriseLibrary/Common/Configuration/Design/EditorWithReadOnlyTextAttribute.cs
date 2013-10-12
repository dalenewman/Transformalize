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
using System.ComponentModel;

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{
    /// <summary>
    /// Attribute that instructs the designtime to make the textbox for a property readonly. <br/>
    /// This property can is used together with an <see cref="EditorAttribute"/>, in which the created text box is readonly, 
    /// though the property can be edited by the editor.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class EditorWithReadOnlyTextAttribute : Attribute
    {
        readonly bool readonlyText;

        /// <summary>
        /// Creates a new instance of <see cref="EditorWithReadOnlyTextAttribute"/>.
        /// </summary>
        /// <param name="readonlyText"><see langword="true"/> if the textbox created for this property should be readonly, otherwise <see langword="false"/>.</param>
        public EditorWithReadOnlyTextAttribute(bool readonlyText)
        {
            this.readonlyText = readonlyText;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the textbox created for this property should be readonly, otherwise <see langword="false"/>.
        /// </summary>
        public bool ReadonlyText
        {
            get { return readonlyText; }
        }
    }
}
