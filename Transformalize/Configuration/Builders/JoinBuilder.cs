namespace Transformalize.Configuration.Builders
{
    public class JoinBuilder {
        private readonly RelationshipBuilder _relationshipBuilder;
        private readonly JoinConfigurationElement _join;

        public JoinBuilder(RelationshipBuilder relationshipBuilder, JoinConfigurationElement joinElement) {
            _relationshipBuilder = relationshipBuilder;
            _join = joinElement;
        }

        public JoinBuilder LeftField(string field) {
            _join.LeftField = field;
            return this;
        }

        public JoinBuilder RightField(string field) {
            _join.RightField = field;
            return this;
        }

        public JoinBuilder Join() {
            return _relationshipBuilder.Join();
        }

        public ProcessConfigurationElement Process() {
            return _relationshipBuilder.Process();
        }
    }
}