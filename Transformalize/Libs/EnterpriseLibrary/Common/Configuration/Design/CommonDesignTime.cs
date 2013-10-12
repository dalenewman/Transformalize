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

namespace Transformalize.Libs.EnterpriseLibrary.Common.Configuration.Design
{
    
    /// <summary>
    /// Class that contains common type names and metadata used by the designtime.
    /// </summary>
    public static class CommonDesignTime
    {
        /// <summary>
        /// Class that contains common command types used by the designtime.
        /// </summary>
        public static class CommandTypeNames
        {
            /// <summary>
            /// Type name of the WizardCommand class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public static string WizardCommand = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.Commands.WizardCommand, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the AddSatelliteProviderCommand class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string AddSatelliteProviderCommand = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.Commands.AddSatelliteProviderCommand, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the AddApplicationBlockCommand class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string AddApplicationBlockCommand = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.Commands.AddApplicationBlockCommand, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the TypePickingCollectionElementAddCommand class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string AddProviderUsingTypePickerCommand = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.TypePickingCollectionElementAddCommand, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the ExportAdmTemplateCommand class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string ExportAdmTemplateCommand = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.ExportAdmTemplateCommand, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the HiddenCommand class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string HiddenCommand = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.Commands.HiddenCommand, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the AddInstrumentationBlockCommand class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string AddInstrumentationApplicationBlockCommand = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.AddInstrumentationBlockCommand, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
        }

        /// <summary>
        /// Class that contains common editor types used by the designtime.
        /// </summary>
        public static class EditorTypes
        {
            /// <summary>
            /// Type name of the DatePickerEditor class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string DatePickerEditor = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Editors.DatePickerEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the ElementCollectionEditor, declared class in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string CollectionEditor = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Editors.ElementCollectionEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the UITypeEditor class, declared in the System.Drawing Assembly.
            /// </summary>
            public const string UITypeEditor = "System.Drawing.Design.UITypeEditor, System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

            /// <summary>
            /// Type name of the TypeSelectionEditor, declared class in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string TypeSelector = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Editors.TypeSelectionEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the FilteredFileNameEditor, declared class in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string FilteredFilePath = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Editors.FilteredFileNameEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the FrameworkElement, declared class in the PresentationFramework Assembly.
            /// </summary>
            public const string FrameworkElement = "System.Windows.FrameworkElement, PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35";

            /// <summary>
            /// Type name of the MultilineTextEditor class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string MultilineText = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Editors.MultilineTextEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the PopupTextEditor class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string PopupTextEditor = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Editors.PopupTextEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the FlagsEditor class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string Flags = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Editors.FlagsEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the RegexTypeEditor class, declared in the System.Design Assembly.
            /// </summary>
            public const string RegexTypeEditor = "System.Web.UI.Design.WebControls.RegexTypeEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

            /// <summary>
            /// Type name of the ConnectionStringEditor class, declared in the System.Design Assembly.
            /// </summary>
            public const string ConnectionStringEditor = "System.Web.UI.Design.ConnectionStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

            /// <summary>
            /// Type name of the TemplateEditor class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string TemplateEditor = "Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Design.Formatters.TemplateEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the IEnvironmentalOverridesEditor interface, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string OverridesEditor = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.IEnvironmentalOverridesEditor, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
        }

        /// <summary>
        /// Class that contains common view model types used by the designtime.
        /// </summary>
        public static class ViewModelTypeNames
        {
            /// <summary>
            /// Type name of the TypeNameProperty class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string TypeNameProperty = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.TypeNameProperty, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the ConfigurationProperty class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string ConfigurationPropertyViewModel =
                "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.ConfigurationProperty, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the SectionViewModel class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string SectionViewModel = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.SectionViewModel, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the CollectionEditorContainedElementProperty class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string CollectionEditorContainedElementProperty =
                "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.CollectionEditorContainedElementProperty, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the CollectionEditorContainedElementReferenceProperty class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string CollectionEditorContainedElementReferenceProperty =
                "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.CollectionEditorContainedElementReferenceProperty, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the RedirectedSectionSourceProperty class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string RedirectedSectionSourceProperty = 
                "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.RedirectedSectionSourceProperty, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
        }

        /// <summary>
        /// Class that contains common converter types used by the designtime runtime.
        /// </summary>
        public static class ConverterTypeNames
        {
            /// <summary>
            /// Type name of the RedirectedSectionNameConverter class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string RedirectedSectionNameConverter =
                "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ComponentModel.Converters.RedirectedSectionNameConverter, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

        }

        /// <summary>
        /// Class that contains common metadata classes used by the designtime.<br/>
        /// This class supports the Enterprise Library infrastructure and is not intended to be used directly from your code.
        /// </summary>
        public static class MetadataTypes
        {
            /// <summary>This class supports the Enterprise Library infrastructure and is not intended to be used directly from your code.</summary>
            [RegisterAsMetadataType(typeof(RedirectedSectionElement))]
            public abstract class RedirectedSectionElementMetadata
            {

                /// <summary>This property supports the Enterprise Library infrastructure and is not intended to be used directly from your code.</summary>
                [TypeConverter(ConverterTypeNames.RedirectedSectionNameConverter)]
                public string Name
                {
                    get;
                    set;
                }
            }
        }

        /// <summary>
        /// Class that contains common validation types used by the designtime.
        /// </summary>
        public static class ValidationTypeNames
        {
            /// <summary>
            /// Type name of the FileWritableValidator class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string FileWritableValidator = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.Validation.FileWritableValidator, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the FilePathValidator class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string FileValidator = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.Validation.FilePathValidator, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the FilePathExistsValidator class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string PathExistsValidator = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.Validation.FilePathExistsValidator, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the RequiredFieldValidator class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string RequiredFieldValidator = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.Validation.RequiredFieldValidator, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the TypeValidator class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string TypeValidator =
                "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.Validation.TypeValidator, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the SelectedSourceValidator class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string SelectedSourceValidator =
                "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.ViewModel.BlockSpecifics.SelectedSourceValidator, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";

            /// <summary>
            /// Type name of the NameValueCollectionValidator class, declared in the Configuration.DesignTime Assembly.
            /// </summary>
            public const string NameValueCollectionValidator = "Microsoft.Practices.EnterpriseLibrary.Configuration.Design.Validation.NameValueCollectionValidator, Microsoft.Practices.EnterpriseLibrary.Configuration.DesignTime";
        }

        /// <summary>
        /// Type names for well known Enterprise Library <see cref="System.Configuration.ConfigurationSection"/> elements.
        /// </summary>
        public static class SectionType
        {
            /// <summary>
            /// Type name for the LoggingSettings section.
            /// </summary>
            public const string LoggingSettings = "Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.LoggingSettings, Microsoft.Practices.EnterpriseLibrary.Logging";

            /// <summary>
            /// Type name for the DatabaseSettings section.
            /// </summary>
            public const string DatabaseSettings = "Microsoft.Practices.EnterpriseLibrary.Data.Configuration.DatabaseSettings, Microsoft.Practices.EnterpriseLibrary.Data";

            /// <summary>
            /// Type name for the ExceptionHandlingSettings section.
            /// </summary>
            public const string ExceptionHandlingSettings = "Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration.ExceptionHandlingSettings, Microsoft.Practices.EnterpriseLibrary.ExceptionHandling";

        }
    }
}
