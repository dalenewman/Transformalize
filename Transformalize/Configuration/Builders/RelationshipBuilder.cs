using Cfg.Net;
using Cfg.Net.Ext;

namespace Transformalize.Configuration.Builders {

    public class RelationshipBuilder {
        private readonly ProcessBuilder _processBuilder;
        private readonly TflRelationship _relationship;

        public RelationshipBuilder(ProcessBuilder processBuilder, TflRelationship relationship) {
            _processBuilder = processBuilder;
            _relationship = relationship;
        }

        public RelationshipBuilder LeftEntity(string entity) {
            _relationship.LeftEntity = entity;
            return this;
        }

        public RelationshipBuilder RightEntity(string entity) {
            _relationship.RightEntity = entity;
            return this;
        }

        public RelationshipBuilder LeftField(string field) {
            _relationship.LeftField = field;
            return this;
        }

        public RelationshipBuilder RightField(string field) {
            _relationship.RightField = field;
            return this;
        }

        public TflProcess Process() {
            return _processBuilder.Process();
        }

        public JoinBuilder Join() {
            var join = _relationship.GetDefaultOf<TflJoin>();
            _relationship.Join.Add(join);
            return new JoinBuilder(this, join);
        }

        public RelationshipBuilder Relationship() {
            return _processBuilder.Relationship();
        }

        public FieldBuilder CalculatedField(string name) {
            return _processBuilder.CalculatedField(name);
        }
    }
}