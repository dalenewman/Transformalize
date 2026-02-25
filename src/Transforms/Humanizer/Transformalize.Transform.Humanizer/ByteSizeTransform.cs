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

using System;
using System.Collections.Generic;
using Humanizer.Bytes;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Humanizer {
   public class ByteSizeTransform : BaseTransform {
      private readonly Field _input;
      private readonly Func<object, ByteSize> _getByteSize;
      private readonly Func<ByteSize, object> _transform;

      public ByteSizeTransform(IContext context = null) : base(context, "double") {
         if (IsMissingContext()) {
            return;
         }

         var received = Received();
         if (received != "string" && received != "bytesize") {
            Context.Error("The bytesize transform expects a string consisting of a number and storage units (e.g. \"1 MB\" or \"200 KB\") or a bytesize.");
         }

         if (Context.Operation.Units == Constants.DefaultSetting) {
            Context.Error("The bytesize transform requires a units parameter so it knows what unit you want the resulting double value in.");
            Run = false;
         }

         _input = SingleInput();

         if(received == "string") {
            _getByteSize = (o) => ByteSize.Parse(o.ToString());
         } else {
            _getByteSize = (o) => (ByteSize)o; 
         }         

         switch (Context.Operation.Units) {
            case "bits":
               _transform = (input) => input.Bits;
               break;
            case "b":
            case "bytes":
               _transform = (input) => input.Bytes;
               break;
            case "kb":
            case "kilobytes":
               _transform = (input) => input.Kilobytes;
               break;
            case "mb":
            case "megabytes":
               _transform = (input) => input.Megabytes;
               break;
            case "gb":
            case "gigabytes":
               _transform = (input) => input.Gigabytes;
               break;
            case "tb":
            case "terabytes":
               _transform = (input) => input.Terabytes;
               break;
            default:
               Context.Warn($"The bytesize transform doesn't understand {Context.Operation.Units} units.  Returning 0.");
               _transform = (input) => 0.0d;
               break;
         }

      }

      public override IRow Operate(IRow row) {
         var input = _getByteSize(row[_input]);
         row[Context.Field] = _transform(input);
         return row;
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         return new[] { new OperationSignature("bytesize") { Parameters = new List<OperationParameter>(1) { new OperationParameter("units") } } };
      }
   }
}