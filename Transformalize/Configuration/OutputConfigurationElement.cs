using System.Configuration;
using Transformalize.Libs.EnterpriseLibrary.Validation.Validators;

namespace Transformalize.Configuration
{
    public class OutputConfigurationElement : ConfigurationElement {

        private const string NAME = "name";
        private const string CONNECTION = "connection";

        //CONDITIONAL
        private const string RUN_FIELD = "run-field";
        private const string RUN_TYPE = "run-type";
        private const string RUN_OPERATOR = "run-operator";
        private const string RUN_VALUE = "run-value";

        [ConfigurationProperty(NAME, IsRequired = true)]
        public string Name {
            get { return this[NAME] as string; }
            set { this[NAME] = value; }
        }

        [ConfigurationProperty(CONNECTION, IsRequired = false)]
        public string Connection {
            get { return this[CONNECTION] as string; }
            set { this[CONNECTION] = value; }
        }

        [ConfigurationProperty(RUN_FIELD, IsRequired = false, DefaultValue = "")]
        public string RunField {
            get { return this[RUN_FIELD] as string; }
            set { this[RUN_FIELD] = value; }
        }

        [ConfigurationProperty(RUN_TYPE, IsRequired = false, DefaultValue = "[default]")]
        public string RunType {
            get { return this[RUN_TYPE] as string; }
            set { this[RUN_TYPE] = value; }
        }

        [EnumConversionValidator(typeof(ComparisonOperator), MessageTemplate = "{1} must be a valid ComparisonOperator. (e.g. Equal, NotEqual, LessThan, LessThanEqual, GreaterThan, GreaterThanEqual)")]
        [ConfigurationProperty(RUN_OPERATOR, IsRequired = false, DefaultValue = "Equal")]
        public string RunOperator {
            get { return this[RUN_OPERATOR] as string; }
            set { this[RUN_OPERATOR] = value; }
        }

        [ConfigurationProperty(RUN_VALUE, IsRequired = false, DefaultValue = "")]
        public string RunValue {
            get { return this[RUN_VALUE] as string; }
            set { this[RUN_VALUE] = value; }
        }

        public override bool IsReadOnly() {
            return false;
        }
    }
}