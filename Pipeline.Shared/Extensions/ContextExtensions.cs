using Pipeline.Configuration;

namespace Pipeline.Contracts {
    public static class ContextExtensions {
        public static Transform PreviousTransform(this IContext context) {
            if (context.Transform.Method == string.Empty)
                return null;

            if (context.Field.Transforms.Count <= 1)
                return null;

            var index = context.Field.Transforms.IndexOf(context.Transform);

            return index == 0 ? null : context.Field.Transforms[index - 1];
        }

        public static Transform NextTransform(this IContext context) {
            if (context.Transform.Method == string.Empty)
                return null;

            if (context.Field.Transforms.Count <= 1)
                return null;

            var last = context.Field.Transforms.Count - 1;
            var index = context.Field.Transforms.IndexOf(context.Transform);

            return index >= last ? null : context.Field.Transforms[index + 1];
        }

    }
}