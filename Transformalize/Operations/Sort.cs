namespace Transformalize.Operations
{
    public class Sort {
        public string Field;
        public string Order;

        public Sort(string field, string order) {
            Field = field;
            Order = order;
        }
    }
}