namespace Transformalize.Configuration.Builders {
    public interface IFieldHolder {
        FieldBuilder CalculatedField(string name);
        FieldBuilder Field(string name);
        EntityBuilder Entity(string name);
        TflProcess Process();
        RelationshipBuilder Relationship();
    }
}