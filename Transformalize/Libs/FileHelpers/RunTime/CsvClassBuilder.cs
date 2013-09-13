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

using Transformalize.Libs.FileHelpers.Engines;
using Transformalize.Libs.FileHelpers.Enums;
using Transformalize.Libs.FileHelpers.ErrorHandling;
using Transformalize.Libs.FileHelpers.Helpers;

namespace Transformalize.Libs.FileHelpers.RunTime
{
    /// <summary>Used to create classes that maps to CSV records (can be quoted, multiplelined quoted, etc).</summary>
    public sealed class CsvClassBuilder : DelimitedClassBuilder
    {
        /// <summary>Creates a new DelimitedClassBuilder.</summary>
        /// <param name="className">The valid class name.</param>
        /// <param name="delimiter">The delimiter for that class.</param>
        /// <param name="sampleFile">A sample file from where to read the field names and number</param>
        public CsvClassBuilder(string className, char delimiter, string sampleFile) : this(new CsvOptions(className, delimiter, sampleFile))
        {
        }

        /// <summary>Creates a new DelimitedClassBuilder.</summary>
        /// <param name="className">The valid class name.</param>
        /// <param name="delimiter">The delimiter for that class.</param>
        /// <param name="numberOfFields">The number of fields in each record.</param>
        public CsvClassBuilder(string className, char delimiter, int numberOfFields) : this(new CsvOptions(className, delimiter, numberOfFields))
        {
        }

        /// <summary>Creates a new DelimitedClassBuilder.</summary>
        /// <param name="options">The specifications for the Csv file.</param>
        public CsvClassBuilder(CsvOptions options) : base(options.RecordClassName, options.Delimiter.ToString())
        {
            IgnoreFirstLines = 1;

            if (options.SampleFileName != string.Empty)
            {
                var firstLine = CommonEngine.RawReadFirstLines(options.SampleFileName, 1);

                if (options.HeaderLines > 0)
                {
                    foreach (var header in firstLine.Split(options.HeaderDelimiter == char.MinValue ? options.Delimiter : options.HeaderDelimiter))
                    {
                        AddField(StringToIdentifier(header));
                    }
                }
                else
                {
                    var fieldsNbr = firstLine.Split(options.Delimiter).Length;
                    for (var i = 0; i < fieldsNbr; i++)
                        AddField(options.FieldsPrefix + i.ToString());
                }
            }
            else if (options.NumberOfFields > 0)
            {
                AddFields(options.NumberOfFields, options.FieldsPrefix);
            }
            else
                throw new BadUsageException("You must provide a SampleFileName or a NumberOfFields to parse a genric CSV file.");
        }

        //private static Regex mRemoveBlanks = new Regex(@"\W", System.Text.RegularExpressions.RegexOptions.Compiled);


        /// <summary>Add a new Delimited field to the current class.</summary>
        /// <param name="fieldName">The Name of the field.</param>
        /// <param name="fieldType">The Type of the field.</param>
        /// <returns>The just created field.</returns>
        public override DelimitedFieldBuilder AddField(string fieldName, string fieldType)
        {
            base.AddField(fieldName, fieldType);

            if (mFields.Count > 1)
            {
                LastField.FieldOptional = true;
                LastField.FieldQuoted = true;
                LastField.QuoteMode = QuoteMode.OptionalForBoth;
                LastField.QuoteMultiline = MultilineMode.AllowForBoth;
            }

            return LastField;
        }


        /// <summary>
        ///     Adds to the class the specified number of fileds.
        /// </summary>
        /// <param name="number">The number of fileds to add.</param>
        public void AddFields(int number)
        {
            AddFields(number, "Field");
        }

        /// <summary>
        ///     Adds to the class the specified number of fileds.
        /// </summary>
        /// <param name="prefix">The prefix used for the fields.</param>
        /// <param name="number">The number of fileds to add.</param>
        public void AddFields(int number, string prefix)
        {
            var initFields = mFields.Count;

            for (var i = 0; i < number; i++)
            {
                var current = i + initFields + 1;
                AddField(prefix + (current).ToString());
            }
        }
    }
}