using Transformalize.Libs.Newtonsoft.Json;

namespace Transformalize.Configuration.Builders
{
    public class JoinBuilder {
        private readonly RelationshipBuilder _relationshipBuilder;
        private readonly TflJoin _join;

        public JoinBuilder(RelationshipBuilder relationshipBuilder, TflJoin joinElement) {
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

        public TflProcess Process() {
            return _relationshipBuilder.Process();
        }
    }
}