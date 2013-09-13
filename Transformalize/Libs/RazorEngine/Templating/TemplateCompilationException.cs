#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;

namespace Transformalize.Libs.RazorEngine.Templating
{
    /// <summary>
    ///     Defines an exception that occurs during compilation of the template.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors"), Serializable]
    public class TemplateCompilationException : Exception
    {
        #region Constructors

        /// <summary>
        ///     Initialises a new instance of <see cref="TemplateCompilationException" />.
        /// </summary>
        /// <param name="errors">The set of compiler errors.</param>
        /// <param name="sourceCode">The source code that wasn't compiled.</param>
        /// <param name="template">The source template that wasn't compiled.</param>
        internal TemplateCompilationException(CompilerErrorCollection errors, string sourceCode, string template)
            : base("Unable to compile template. " + errors[0].ErrorText + "\n\nOther compilation errors may have occurred. Check the Errors property for more information.")
        {
            var list = errors.Cast<CompilerError>().ToList();
            Errors = new ReadOnlyCollection<CompilerError>(list);
            SourceCode = sourceCode;
            Template = template;
        }

        /// <summary>
        ///     Initialises a new instance of <see cref="TemplateCompilationException" /> from serialised data.
        /// </summary>
        /// <param name="info">The serialisation info.</param>
        /// <param name="context">The streaming context.</param>
        protected TemplateCompilationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            var count = info.GetInt32("Count");

            var list = new List<CompilerError>();
            var type = typeof (CompilerError);

            for (var i = 0; i < count; i++)
            {
                list.Add((CompilerError) info.GetValue("Errors[" + i + "]", type));
            }

            Errors = new ReadOnlyCollection<CompilerError>(list);

            SourceCode = info.GetString("SourceCode");
            Template = info.GetString("Template");
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the set of compiler errors.
        /// </summary>
        public ReadOnlyCollection<CompilerError> Errors { get; private set; }

        /// <summary>
        ///     Gets the source code that wasn't compiled.
        /// </summary>
        public string SourceCode { get; private set; }

        /// <summary>
        ///     Gets the source template that wasn't compiled.
        /// </summary>
        public string Template { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the object data for serialisation.
        /// </summary>
        /// <param name="info">The serialisation info.</param>
        /// <param name="context">The streaming context.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Count", Errors.Count);

            for (var i = 0; i < Errors.Count; i++)
                info.AddValue("Errors[" + i + "]", Errors[i]);

            info.AddValue("SourceCode", SourceCode ?? string.Empty);
            info.AddValue("Template", Template ?? string.Empty);
        }

        #endregion
    }
}