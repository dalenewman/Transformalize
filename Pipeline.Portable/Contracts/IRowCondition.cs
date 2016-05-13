namespace Pipeline.Contracts {
    public interface IRowCondition {
        bool Eval(IRow row);
    }
}