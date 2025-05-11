#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2025 Dale Newman
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
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms.Dates {

   public class TimeAgoTransform : RelativeTimeTransform {
      public TimeAgoTransform(IContext context = null) : base(context) { }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("timeago") {
            Parameters = new List<OperationParameter> {
                    new OperationParameter("from-time-zone", "UTC")
                }
         };
      }
   }

   public class TimeAheadTransform : RelativeTimeTransform {
      public TimeAheadTransform(IContext context = null) : base(context) {
      }

      public override IEnumerable<OperationSignature> GetSignatures() {
         yield return new OperationSignature("timeahead") {
            Parameters = new List<OperationParameter> {
                    new OperationParameter("from-time-zone", "UTC")
                }
         };
      }
   }

   public class RelativeTimeTransform : BaseTransform {

      private readonly long _nowTicks;
      private readonly Field _input;
      private const double Second = 1.0;
      private const double Minute = 60.0 * Second;
      private const double Hour = 60.0 * Minute;
      private const double Day = 24.0 * Hour;
      private const double Month = (365.0 / 12.0) * Day;

      public RelativeTimeTransform(IContext context) : base(context, "string") {
         if (IsMissingContext()) {
            return;
         }

         if (IsNotReceiving("date")) {
            return;
         }

         _input = SingleInput();
         var fromTimeZone = context.Operation.FromTimeZone == Constants.DefaultSetting ? "UTC" : context.Operation.FromTimeZone;

         _nowTicks = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, fromTimeZone).Ticks;
      }

      public override IRow Operate(IRow row) {
         row[Context.Field] = GetRelativeTime(_nowTicks, ((DateTime)row[_input]).Ticks);

         return row;
      }

      public static string GetRelativeTime(long nowTicks, long thenTicks) {

         var suffix = nowTicks > thenTicks ? " ago" : string.Empty;
         var ts = nowTicks > thenTicks ? new TimeSpan(nowTicks - thenTicks) : new TimeSpan(thenTicks - nowTicks);
         var delta = Math.Abs(ts.TotalSeconds);

         if (delta < 1.0 * Minute) {
            return ts.Seconds == 1 ? "one second" + suffix : ts.Seconds + " seconds" + suffix;
         }
         if (delta < 2.0 * Minute) {
            return "a minute" + suffix;
         }
         if (delta < 45.0 * Minute) {
            return ts.Minutes + " minutes" + suffix;
         }
         if (delta < 90.0 * Minute) {
            return "an hour" + suffix;
         }
         if (delta < 24.0 * Hour) {
            return Math.Max(ts.Hours, 2) + " hours" + suffix;
         }
         if (delta < 2.0 * Day) {
            return nowTicks > thenTicks ? "yesterday" : "tomorrow";
         }
         if (delta < 30.0 * Day) {
            return ts.Days + " days" + suffix;
         }
         if (delta < 12.0 * Month) {
            var months = Math.Max(Convert.ToInt32(Math.Floor(ts.TotalDays / 30.0)), 1);
            return months <= 1 ? "one month" + suffix : months + " months" + suffix;
         }

         var years = Math.Max(Convert.ToInt32(Math.Floor(ts.TotalDays / 365.0)), 1);
         return years <= 1 ? "one year" + suffix : years + " years" + suffix;
      }

   }
}