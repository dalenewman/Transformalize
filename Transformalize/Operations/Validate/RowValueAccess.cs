using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;
using Transformalize.Libs.Rhino.Etl;

namespace Transformalize.Operations.Validate
{
    public class RowValueAccess : ValueAccess {
        private readonly string _inKey;

        public RowValueAccess(string inKey) {
            _inKey = inKey;
        }

        public override bool GetValue(object source, out object value, out string valueAccessFailureMessage) {
            value = ((Row)source)[_inKey];
            valueAccessFailureMessage = string.Empty;
            return true;
        }

        public override string Key {
            get { return _inKey; }
        }
    }
}