namespace Transformalize.Contracts {
    public interface IValidate : IOperation {
        IField ValidField { get; }
        IField MessageField { get; }
    }
}