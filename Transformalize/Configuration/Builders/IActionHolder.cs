namespace Transformalize.Configuration.Builders
{
    public interface IActionHolder
    {
        ActionBuilder Action(string action);
        TemplateBuilder Template(string name);
        EntityBuilder Entity(string name);
        SearchTypeBuilder SearchType(string name);
        MapBuilder Map(string name);
        ProcessBuilder TemplatePath(string path);
        ProcessBuilder ScriptPath(string path);
    }
}