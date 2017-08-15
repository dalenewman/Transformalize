#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Extensions {
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