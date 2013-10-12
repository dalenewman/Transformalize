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
    /// Attribute class that can be oved to offer a properties add-commands to the containing Element View Model.<br/>
    /// This can be usefull for properties that contain a collection of providers, of which the Element Collection View Model is not shown in the UI (User Interface).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=true)]
    public class PromoteCommandsAttribute : Attribute
    {
    }
}
