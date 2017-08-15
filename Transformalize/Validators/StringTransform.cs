namespace Pipeline.Shared {
    public abstract class StringTransform : BaseTransform {

        protected readonly Func<IRow, Field, string> GetString;

        protected StringTransform(IContext context, string returns) : base(context, returns) {
            GetString = delegate (IRow row, Field field) {
                if (Received() == "string") {
                    return (string)row[field];  // cast
                }
                return row[field].ToString();  // conversion, assumed to be more expensive
            };
        }
    }
}