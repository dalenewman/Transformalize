using System;
using System.Linq;
using FileHelpers;
using FileHelpers.Dynamic;
using Transformalize.Context;

namespace Transformalize.Provider.File {
    public static class FileHelpersEngineFactory {

        public static FileHelperAsyncEngine Create(OutputContext context) {

            var delimiter = string.IsNullOrEmpty(context.Connection.Delimiter) ? "," : context.Connection.Delimiter;

            var builder = new DelimitedClassBuilder(Utility.Identifier(context.Entity.OutputTableName(context.Process.Name))) {
                IgnoreEmptyLines = true,
                Delimiter = delimiter,
                IgnoreFirstLines = 0
            };

            foreach (var field in context.OutputFields) {
                var fieldBuilder = builder.AddField(field.FieldName(), typeof(string));
                fieldBuilder.FieldQuoted = true;
                fieldBuilder.QuoteChar = context.Connection.TextQualifier;
                fieldBuilder.QuoteMode = QuoteMode.OptionalForBoth;
                fieldBuilder.FieldOptional = field.Optional;
            }

            FileHelpers.ErrorMode errorMode;
            Enum.TryParse(context.Connection.ErrorMode, true, out errorMode);

            FileHelperAsyncEngine engine;

            if (context.Connection.Header == Constants.DefaultSetting) {
                var headerText = string.Join(delimiter, context.OutputFields.Select(f => f.Label.Replace(delimiter, " ")));
                engine = new FileHelperAsyncEngine(builder.CreateRecordClass()) {
                    ErrorMode = errorMode,
                    HeaderText = headerText,
                    FooterText = context.Connection.Footer
                };
            } else {
                engine = new FileHelperAsyncEngine(builder.CreateRecordClass()) { ErrorMode = errorMode };
            }

            return engine;

        }

        public static FileHelperAsyncEngine Create(InputContext context) {

            var identifier = Utility.Identifier(context.Entity.OutputTableName(context.Process.Name));
            var delimiter = string.IsNullOrEmpty(context.Connection.Delimiter) ? "," : context.Connection.Delimiter;

            var builder = new DelimitedClassBuilder(identifier) {
                IgnoreEmptyLines = true,
                Delimiter = delimiter,
                IgnoreFirstLines = context.Connection.Start
            };

            foreach (var field in context.InputFields) {
                var fieldBuilder = builder.AddField(field.FieldName(), typeof(string));
                fieldBuilder.FieldQuoted = true;
                fieldBuilder.QuoteChar = context.Connection.TextQualifier;
                fieldBuilder.QuoteMode = QuoteMode.OptionalForBoth;
                fieldBuilder.FieldOptional = field.Optional;
            }

            FileHelpers.ErrorMode errorMode;
            Enum.TryParse(context.Connection.ErrorMode, true, out errorMode);

            var engine = new FileHelperAsyncEngine(builder.CreateRecordClass());
            engine.ErrorManager.ErrorMode = errorMode;
            engine.ErrorManager.ErrorLimit = context.Connection.ErrorLimit;

            return engine;
        }
    }
}
