using System;
using Transformalize.Contracts;

namespace Transformalize.Validators {

    public abstract class StringValidate : BaseValidate {

        protected readonly Func<IRow, IField, string> GetString;

        protected StringValidate(IContext context) : base(context) {
            GetString = delegate (IRow row, IField field) {
                if (SingleInput().Type == "string") {
                    return (string)row[field];  // cast
                }
                return row[field].ToString();  // conversion, assumed to be more expensive
            };
            if (MessageField == null) {
                AppendMessage = delegate { };
            } else {
                AppendMessage = delegate (IRow row, string message) {
                    row[MessageField] = GetString(row, MessageField) + message + "|";
                };
            }
        }
    }
}