namespace Transformalize.Configuration
{
    public interface IField {
        string Schema { get; }
        string Entity { get; }
        string Parent { get; }
        string Name { get; }
        string Type { get; }
        string Alias { get; }
        int Length { get; }
        int Precision { get; }
        int Scale { get; }
        bool Output { get; }
        FieldType FieldType { get; }
        string SqlDataType();
    }
}